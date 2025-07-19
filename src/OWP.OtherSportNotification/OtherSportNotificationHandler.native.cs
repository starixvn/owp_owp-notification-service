using System.Globalization;
using MediatR;
using Microsoft.Extensions.Configuration;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Models;
using OWP.OtherSportNotification.Services;
using System.Text.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using System.Net.Http.Json;
using Fanex.Logging;
using Fanex.Caching;
using Newtonsoft.Json;

namespace OWP.OtherSportNotification;

public class RocketChatAuthResponse
{
    public string Status { get; set; }
    public AuthData Data { get; set; }

    public class AuthData
    {
        public string AuthToken { get; set; }
        public string UserId { get; set; }
    }
}

public class RCResponse
{
    public bool Success { get; set; }
    public List<string> Message { get; set; } = [];
}

public class LastUpdateInfo
{
    public DateTime Date { get; set; }

    public string Username { get; set; }
}

public class OtherSportNotificationHandler(
    ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
    AssigneeChangedValueFilter assigneeChangedValueFilter,
    ChangedValueAggregator changedValueAggregator,
    MessageFactory messageFactory,
    RocketChatMessageFactory rocketChatMessageFactory,
    OtherSportRepository repository,
    NotificationService notificationService,
    IncludeBBSectionService bBSectionService,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger logger,
    ICache cache)
    : IRequestHandler<OtherSportNotificationRequest, Unit>
{
    private const string ChannelName = "#LC_Update_Notification";
    private const int maxRetries = 3;
    private const int retryDelaySeconds = 2;
    private const string KEY_OS_UPLOAD_LAST_UPDATED = "OWP-OS-AssignmentUpload-Last-Updated";

    public async Task<Unit> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);

        await repository.InsertMessage(messageModels, request.SessionId, request.ActorId);

        await notificationService.Notify(messageModels);

        await bBSectionService.InsertMessage(messages, request);

        await SendMessageToRc(aggregatedChangedValues, request.UserName, request.SessionId);
        return Unit.Value;
    }

    private async Task SendMessageToRc(IList<ChangedValue> changedValues, string userName, string sessionId)
    {
        try
        {
            var client = CreateClient();
            await LoginToRc(client);
            await MessageToChannel(client, userName, sessionId);
            await MessageToTrader(client, changedValues, userName, sessionId);
            await LogOutRC(client);
        }
        catch (Exception e)
        {
            await logger.ErrorAsync(e.Message, e);
        }
    }

    private HttpClient CreateClient()
    {
        var chatUrl = configuration["ChatServer:Url"];
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(chatUrl);
        client.Timeout = TimeSpan.FromMinutes(2);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        return client;
    }

    private static async Task LogOutRC(HttpClient client)
    {
        await client.PostAsync("api/v1/logout", null);
    }

    public async Task<string> GetLastUpdate()
    {
        var (_, value) = await cache.TryGetAsync<string>(KEY_OS_UPLOAD_LAST_UPDATED);

        var lastUpdated = string.IsNullOrEmpty(value)
            ? new LastUpdateInfo()
            : JsonConvert.DeserializeObject<LastUpdateInfo>(value);

        return lastUpdated.Date.ToString("MMMM dd, yyyy hh:mm tt");
    }

    private async Task LoginToRc(HttpClient client)
    {
        try
        {
            var username = configuration["ChatServer:BotName"];
            var password = configuration["ChatServer:BotPassword"];
            var loginPayload = new { user = username, password = password };

            var response = await client.PostAsJsonAsync("api/v1/login", loginPayload);
            response.EnsureSuccessStatusCode();

            var authResult = await response.Content.ReadFromJsonAsync<RocketChatAuthResponse>();

            client.DefaultRequestHeaders.Add("X-Auth-Token", authResult.Data.AuthToken);
            client.DefaultRequestHeaders.Add("X-User-Id", authResult.Data.UserId);
        }
        catch (Exception exception)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]- Login - Fatal error", exception.ToString());
           
            throw new Exception("Failed to login to Rocket.Chat", exception);
        }
       
    }
    private async Task MessageToTrader(
        HttpClient client, 
        IList<ChangedValue> changedValues, 
        string uploadBy,
        string sessionId)
    {
        try
        {
            if (!changedValues.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Success", "Nothing changed");
                return;
            }

            var messageModels = await rocketChatMessageFactory.BuildMessage(changedValues);
            var rcResponseSuccess = new List<object>();
            var rcResponseFail = new List<object>();

            foreach (var message in messageModels)
            {
                var messageResponse = await SendMessageWithRetry(client, message);
                if (messageResponse.Success)
                {
                    rcResponseSuccess.Add(new
                    {
                        recipient = message.Recipient,
                        message = messageResponse,
                    });
                }
                else
                {
                    rcResponseFail.Add(new
                    {
                        recipient = message.Recipient,
                        message = messageResponse,
                    });
                }
                
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
           
            await LogRcResponse("[OsNotification]-MessageToTrader - Success", rcResponseSuccess, uploadBy, sessionId);
            await LogRcResponse("[OsNotification]-MessageToTrader - Fail", rcResponseFail, uploadBy, sessionId);
        }
        catch (Exception ex)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Fatal error", ex.ToString());
            throw new Exception("[OsNotification]-MessageToTrader - Fatal error", ex);
        }
    }

    private async Task LogRcResponse(
        string action, IList<object> payload, string uploadBy, string sessionId)
    {
        if (payload.Any())
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), action,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    sessionId,
                    uploadBy,
                    payload
                }));
        }
       
    }
    private static async Task<RCResponse> SendMessageWithRetry(
        HttpClient client, 
        RocketChatMessageModel message)
    {
        var imCreateResponse = await client.PostAsJsonAsync("api/v1/im.create", new { username = message.Recipient });
        var imResult = await imCreateResponse.Content.ReadFromJsonAsync<JsonElement>();
        var roomId = imResult.GetProperty("room").GetProperty("_id").GetString();

        var rcResponse = new RCResponse();
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var messagePayload = new { roomId = roomId, text = message.Message };
                var response = await client.PostAsJsonAsync("api/v1/chat.postMessage", messagePayload);

                if (response.IsSuccessStatusCode)
                {
                    rcResponse.Success = true;
                    rcResponse.Message.Add("Send message to trader successfully");
                    return rcResponse;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                rcResponse.Success = false;
                rcResponse.Message.Add($"Attempt {attempt} failed to send message reason: {responseBody}");

            }
            catch (Exception ex)
            {
                var exMessage = $"Exception: {ex.ToString()}, message info: {System.Text.Json.JsonSerializer.Serialize(message)}";
                rcResponse.Success = false;
                rcResponse.Message.Add($"Attempt {attempt} exception during message send reason: {exMessage}");
            }
            if (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
            }
        }

        return rcResponse;
    }

    private async Task MessageToChannel(HttpClient client, string userName, string sessionId)
    {
        try
        {
            var messageResponse = await SendChannelMessageWithRetry(client, userName);
            var logInfo=new
            {
                sessionId = sessionId,
                uploadBy = userName,
                channel = ChannelName,
                message = messageResponse
            };

            var logAction= "[OsNotification]-MessageToChannel: LC_Update_Notification - Success";
            if (!messageResponse.Success)
            {
                logAction= "[OsNotification]-MessageToChannel: LC_Update_Notification - Fail";
            }
            await LogDetailedPayload(ToGmt8(DateTime.Now), logAction,
                System.Text.Json.JsonSerializer.Serialize(logInfo));
        }
        catch (Exception ex)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex.ToString());
            throw new Exception("[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex);
        }
    }

    private async Task<RCResponse> SendChannelMessageWithRetry(
        HttpClient client,
        string userName)
    {
        var rcResponse = new RCResponse();
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            var message = new
            {
                channel = ChannelName,
                text = $"Hi all,\nLC updated (Last updated on **{await GetLastUpdate()} (GMT+8)** by **{userName}**).\nPlease download the LC latest version.\nThanks"
            };

            try
            {
                var response = await client.PostAsJsonAsync("api/v1/chat.postMessage", message);

                if (response.IsSuccessStatusCode)
                {
                    rcResponse.Success = true;
                    rcResponse.Message.Add("Send message to trader successfully");
                    return rcResponse;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                rcResponse.Success = false;
                rcResponse.Message.Add($"Attempt {attempt} failed to send message reason: {responseBody}");
            }
            catch (Exception ex)
            {
                var exMessage = $"Exception: {ex.ToString()}, message info: {System.Text.Json.JsonSerializer.Serialize(message)}";
                rcResponse.Success = false;
                rcResponse.Message.Add($"Attempt {attempt} exception during message send reason: {exMessage}");
            }

            if (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
            }
        }

        return rcResponse;
    }

    private async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
    {
        await repository.WriteLog(new OwpLogCriteria()
        {
            CreatedDate = CreatedDate,
            Action = Action,
            Payload = Payload
        });
    }

    private static DateTime ToGmt8(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().AddHours(8);
    }
}
