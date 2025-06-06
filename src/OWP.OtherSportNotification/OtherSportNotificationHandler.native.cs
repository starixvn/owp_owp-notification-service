using System.Globalization;
using MediatR;
using Microsoft.Extensions.Configuration;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Models;
using OWP.OtherSportNotification.Services;
using System.Text;
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

        await bBSectionService.InsertMessage(messages,request);
        await MessageToChannel(request.UserName, cancellationToken);
        await MessageToTrader(aggregatedChangedValues, cancellationToken);
        return Unit.Value;
    }



    private async Task MessageToTrader(IList<ChangedValue> changedValue,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!changedValue.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), $"[OsNotification]-MessageToTrader - Success", "Nothing change");

                return;
            }
            var msgModel= await rocketChatMessageFactory.BuildMessage(changedValue);

            foreach (var msg in msgModel)
            {
                var payload = new
                {
                    channel = $"@{msg.Recipient}",
                    text = msg.Message
                };

                using var client = httpClientFactory.CreateClient("OWPChatBot");
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/v1/chat.postMessage", content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to send message to chat server: {System.Text.Json.JsonSerializer.Serialize(response)}");
                }
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                await LogDetailedPayload(ToGmt8(DateTime.Now), $"[OsNotification]-MessageToTrader {msg.Recipient}-Success", responseBody);
            }
        }
        catch (Exception e)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), $"[OsNotification]-MessageToTrader - fail", e.Message);
        }
    }
    private async Task MessageToChannel(string userName, CancellationToken cancellationToken)
    {
        try
        {
            var payload = new
            {
                text = $"Hi all,\nLC updated (Last updated on **[{ToGmt8(DateTime.Now).ToString("MMMM dd, yyyy hh:mm tt", CultureInfo.InvariantCulture)}] (GMT+8)** by **[{userName}]**).\nPlease download the LC latest version.\nThanks"
            };
            var webHook = configuration["ChatServer:WebHook"];
            using var client = httpClientFactory.CreateClient("OWPChatAdmin");
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(webHook, content,cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send message to channel LC_Update_Notification: {System.Text.Json.JsonSerializer.Serialize(response)}");
            }
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Success", responseBody);
        }
        catch (Exception e)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-fail", e.Message);
        }

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
