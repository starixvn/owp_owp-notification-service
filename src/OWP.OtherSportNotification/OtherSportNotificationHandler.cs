using MediatR;
using Microsoft.AspNetCore.SignalR;
using OWP.Core.Hubs;
using OWP.Core.NotificationRequests;
using OWP.OtherSportNotification.Models;
using OWP.OtherSportNotification.Services;

namespace OWP.OtherSportNotification;

public class OtherSportNotificationHandler : IRequestHandler<OtherSportNotificationRequest, Unit>
{
    private readonly ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase;
    private readonly AssigneeChangedValueFilter assigneeChangedValueFilter;
    private readonly ChangedValueAggregator changedValueAggregator;
    private readonly MessageFactory messageFactory;
    private readonly OtherSportRepository repository;
    private readonly IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> notifyHub;

    public OtherSportNotificationHandler(ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
        AssigneeChangedValueFilter assigneeChangedValueFilter,
        ChangedValueAggregator changedValueAggregator,
        MessageFactory messageFactory,
        OtherSportRepository repository,
        IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> notifyHub)
    {
        this.changedValueByCacheAndDatabase = changedValueByCacheAndDatabase;
        this.assigneeChangedValueFilter = assigneeChangedValueFilter;
        this.changedValueAggregator = changedValueAggregator;
        this.messageFactory = messageFactory;
        this.repository = repository;
        this.notifyHub = notifyHub;
    }

    public async Task<Unit> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate); var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues);

        await repository.InsertMessage(messageModels, request.SessionId, 8);

        await Notify(messageModels);

        return Unit.Value;
    }

    public async Task Notify(List<MessageModel> messages)
    {
        foreach (var message in messages)
        {
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message
            };

            await notifyHub.Clients
               //.Users(message.ReceivedUserIds)
               .All
               .notifyChangedInformationData(result);
        }
    }
}
