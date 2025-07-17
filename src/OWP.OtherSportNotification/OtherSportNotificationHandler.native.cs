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
    ILogger logger)
    : IRequestHandler<OtherSportNotificationRequest, Unit>
{
    private const string ChannelName = "#LC_Update_Notification";
    private const int maxRetries = 3;
    private const int retryDelaySeconds = 2;
    public async Task<Unit> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);

        await repository.InsertMessage(messageModels, request.SessionId, request.ActorId);

        await notificationService.Notify(messageModels);

        await bBSectionService.InsertMessage(messages, request);

        await SendMessageToRc(aggregatedChangedValues, request.UserName);
        return Unit.Value;
    }

    private async Task SendMessageToRc(IList<ChangedValue> changedValues, string userName)
    {
        try
        {
            var client = CreateClient();
            await LoginToRc(client);
            await MessageToChannel(client, userName);
            await MessageToTrader(client, changedValues, userName);
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
    private async Task MessageToTrader(HttpClient client, IList<ChangedValue> changedValues, string uplodBy)
    {
        try
        {
            if (!changedValues.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Success", "Nothing changed");
                return;
            }

            var messageModels = await rocketChatMessageFactory.BuildMessage(changedValues);
            var rcResponse = new List<object>();

            foreach (var message in messageModels)
            {
                var messageResponse = await SendMessageWithRetry(client, message);

                rcResponse.Add(new
                {
                    uploadBy= uplodBy,
                    user = message.Recipient,
                    message = messageResponse,
                });

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader",
                JsonSerializer.Serialize(rcResponse));
        }
        catch (Exception ex)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Fatal error", ex.ToString());
            throw new Exception("[OsNotification]-MessageToTrader - Fatal error", ex);
        }
    }

    private static async Task<RCResponse> SendMessageWithRetry(HttpClient client, RocketChatMessageModel message)
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
                var exMessage = $"Exception: {ex.ToString()}, message info: {JsonSerializer.Serialize(message)}";
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

    private async Task MessageToChannel(HttpClient client, string userName)
    {
        try
        {
            var rcResponse = new List<object>();
            var messageResponse = await SendChannelMessageWithRetry(client, userName);
            rcResponse.Add(new
            {
                uploadBy = userName,
                channel = ChannelName,
                message = messageResponse
            });
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal",
                JsonSerializer.Serialize(rcResponse));
        }
        catch (Exception ex)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex.ToString());
            throw new Exception("[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex);
        }
    }

    private static async Task<RCResponse> SendChannelMessageWithRetry(
        HttpClient client,
        string userName)
    {
        var rcResponse = new RCResponse();
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            var message = new
            {
                channel = ChannelName,
                text = $"Hi all,\nLC updated (Last updated on **{ToGmt8(DateTime.Now).ToString("MMMM dd, yyyy hh:mm tt", CultureInfo.InvariantCulture)} (GMT+8)** by **{userName}**).\nPlease download the LC latest version.\nThanks"
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
                var exMessage = $"Exception: {ex.ToString()}, message info: {JsonSerializer.Serialize(message)}";
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
