namespace OWP.AssignmentUploadNotification;

using Core.NotificationRequests.AssignmentUpload;
using MediatR;
using OWP.Core.Models;
using OWP.Core.Services;
using Services;
using System;
using System.Linq;
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
    WatcherChangedValueGenerator watcherChangedValueGenerator,
    ChangedValueAggregator changedValueAggregator,
    MessageFactory messageFactory,
    CommandRepository commandRepository,
    NotificationService notificationService)
    : IRequestHandler<AssignmentUploadNotifyRequest, Unit>
{
    public async Task<Unit> Handle(AssignmentUploadNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.EventDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var watcherChangedValues = await watcherChangedValueGenerator.Create(messages, assigneeChangedValues);
        var allChangedValues = watcherChangedValues.Union(assigneeChangedValues).ToList();
        var aggregatedChangedValues = changedValueAggregator.Aggregate(allChangedValues);
        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName).ToList();
        await commandRepository.InsertMessage(messageModels, request.SessionId, request.ActorId);
        var messageSender = BuildMessageSender(messageModels);
        await notificationService.Notify(messageSender);

        return Unit.Value;
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
}