using MediatR;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Services;

namespace OWP.OtherSportNotification;

public class OtherSportNotificationHandler(
    ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
    AssigneeChangedValueFilter assigneeChangedValueFilter,
    ChangedValueAggregator changedValueAggregator,
    MessageFactory messageFactory,
    OtherSportRepository repository,
    NotificationService notificationService,
    IncludeBBSectionService bBSectionService)
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
        
        return Unit.Value;
    }
}
