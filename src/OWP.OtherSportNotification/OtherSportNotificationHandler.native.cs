using System.Globalization;
using MediatR;
using Microsoft.Extensions.Configuration;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Models;
using OWP.OtherSportNotification.Services;
using System.Text;
using System.Text.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;

namespace OWP.OtherSportNotification;

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
    IConfiguration configuration)
    : IRequestHandler<OtherSportNotificationRequest, Unit>
{
    public async Task<Unit> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);

        await repository.InsertMessage(messageModels, request.SessionId, request.ActorId);

        await notificationService.Notify(messageModels);

        await bBSectionService.InsertMessage(messages, request);
        await MessageToChannel(request.UserName, cancellationToken);
        await MessageToTrader(aggregatedChangedValues, cancellationToken);
        return Unit.Value;
    }

    private async Task MessageToTrader(IList<ChangedValue> changedValues, CancellationToken cancellationToken)
    {
        try
        {
            if (!changedValues.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Success", "Nothing changed");
                return;
            }

            var messageModels = await rocketChatMessageFactory.BuildMessage(changedValues);
            var failedMessages = new List<object>();

            using var client = httpClientFactory.CreateClient("OWPChatBot");

            foreach (var message in messageModels)
            {
                var wasSent = await SendMessageWithRetry(client, message, cancellationToken);

                if (!wasSent)
                {
                    failedMessages.Add(CreateMessagePayload(message));
                }

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }

            if (failedMessages.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Failed messages",
                    JsonSerializer.Serialize(failedMessages));
            }
        }
        catch (Exception ex)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Fatal error", ex.ToString());
        }
    }

    private async Task<bool> SendMessageWithRetry(HttpClient client, RocketChatMessageModel message, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        const int retryDelaySeconds = 2;

        var payload = CreateMessagePayload(message);

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var content = CreateHttpContent(payload);

                var response = await client.PostAsync("api/v1/chat.postMessage", content, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    await LogDetailedPayload(ToGmt8(DateTime.Now),
                        $"[OsNotification]-MessageToTrader {message.Recipient}-Success", responseBody);
                    return true;
                }

                await LogDetailedPayload(ToGmt8(DateTime.Now),
                    $"[OsNotification]-MessageToTrader - Attempt {attempt} failed to send message", responseBody);
            }
            catch (Exception ex)
            {
                var logMessage = $"Exception: {ex.ToString()}, Payload: {JsonSerializer.Serialize(payload)}";
                await LogDetailedPayload(ToGmt8(DateTime.Now),
                    $"[OsNotification]-MessageToTrader - Attempt {attempt} exception during message send", logMessage);
            }

            if (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), cancellationToken);
            }
        }

        return false;
    }

    private static object CreateMessagePayload(RocketChatMessageModel message)
    {
        return new
        {
            channel = $"@{message.Recipient}",
            text = message.Message
        };
    }

    private static StringContent CreateHttpContent(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task MessageToChannel(string userName, CancellationToken cancellationToken)
    {
        try
        {
            var payload = CreateChannelMessagePayload(userName);
            var webHook = configuration["ChatServer:WebHook"];

            using var client = httpClientFactory.CreateClient("OWPChatAdmin");

            var wasSent = await SendChannelMessageWithRetry(client, webHook, payload, cancellationToken);

            if (!wasSent)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Failed",
                    "All retry attempts failed");
            }
        }
        catch (Exception ex)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex.ToString());
        }
    }

    private async Task<bool> SendChannelMessageWithRetry(HttpClient client, string webHook, object payload, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        const int retryDelaySeconds = 2;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var content = CreateHttpContent(payload);
                var response = await client.PostAsync(webHook, content, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Success", responseBody);
                    return true;
                }

                await LogDetailedPayload(ToGmt8(DateTime.Now),
                    $"[OsNotification]-MessageToChannel: LC_Update_Notification - Attempt {attempt} failed", responseBody);
            }
            catch (Exception ex)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now),
                    $"[OsNotification]-MessageToChannel: LC_Update_Notification - Attempt {attempt} exception", ex.ToString());
            }

            if (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), cancellationToken);
            }
        }

        return false;
    }

    private static object CreateChannelMessagePayload(string userName)
    {
        return new
        {
            text = $"Hi all,\nLC updated (Last updated on **{ToGmt8(DateTime.Now).ToString("MMMM dd, yyyy hh:mm tt", CultureInfo.InvariantCulture)} (GMT+8)** by **{userName}**).\nPlease download the LC latest version.\nThanks"
        };
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
