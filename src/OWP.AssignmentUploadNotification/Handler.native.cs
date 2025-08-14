using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.AssignmentUploadNotification.Models;

namespace OWP.AssignmentUploadNotification;

using Core.NotificationRequests.AssignmentUpload;
using MediatR;
using OWP.Core.Models;
using OWP.Core.Services;
using Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class NotifyRequest : IRequest<bool>
{
    public string SessionId { get; set; }

    public DateTime EventDate { get; set; }

    public bool IsUploadFMAndAIO { get; set; }

    public string UserName { get; set; }

    public int ActorId { get; set; }
}

public class Handler(
    ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
    AssigneeChangedValueFilter assigneeChangedValueFilter,
    ChangedValueAggregator changedValueAggregator,
    MessageFactory messageFactory,
    CommandRepository commandRepository,
    NotificationService notificationService,
    IHttpClientFactory httpClientFactory)
    : IRequestHandler<AssignmentUploadNotifyRequest, Unit>
{
    public async Task<Unit> Handle(AssignmentUploadNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.EventDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);
        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName).ToList();
        await commandRepository.InsertMessage(messageModels, request.SessionId, request.ActorId);
        var messageSender = BuildMessageSender(messageModels);
        await notificationService.Notify(messageSender);
        //await MessageToTrader(messageModels, cancellationToken);

        return Unit.Value;
    }

    private async Task MessageToTrader(IList<Models.MessageModel> messageModels,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!messageModels.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[AssignmentUpload]-MessageToTrader - Success", "Nothing change");
                return;
            }

            using var client = httpClientFactory.CreateClient("OWPChatBot");

            var sendTasks = messageModels.SelectMany(msg =>
            {
                var receivedUsernames = msg.ReceivedUserNameList.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return receivedUsernames.Select(async receivedUserName =>
                {
                    try
                    {
                        var payload = new
                        {
                            channel = $"@{receivedUserName}",
                            text = msg.MessageContent
                        };

                        var json = System.Text.Json.JsonSerializer.Serialize(payload);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("api/v1/chat.postMessage", content, cancellationToken).ConfigureAwait(false);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Failed to send message to chat server: {response.StatusCode}");
                        }

                        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        await LogDetailedPayload(ToGmt8(DateTime.Now), $"[AssignmentUpload]-MessageToTrader {receivedUserName} - Success", responseBody).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await LogDetailedPayload(ToGmt8(DateTime.Now), $"[AssignmentUpload]-MessageToTrader {receivedUserName} - Error", ex.Message).ConfigureAwait(false);
                    }
                });
            });

            await Task.WhenAll(sendTasks);
        }
        catch (Exception e)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[AssignmentUpload]-MessageToTrader - fail", e.Message);
        }

    }

    private async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
    {
        await commandRepository.WriteLog(new OwpLogCriteria()
        {
            CreatedDate = CreatedDate,
            Action = Action,
            Payload = Payload
        });
    }

    private static IList<MessageModelCommon<MessageModel>> BuildMessageSender(IEnumerable<Models.MessageModel> messageModels)
        => messageModels.Select(m => new MessageModelCommon<MessageModel>
        {
            Message = new MessageModel
            {
                MessageContent = m.MessageContent,
                MessageType = m.MessageType,
                IsImportant = m.IsImportant
            },
            ReceivedUserIds = m.ReceivedUserIds
        }).ToList();

    private static DateTime ToGmt8(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().AddHours(8);
    }
}