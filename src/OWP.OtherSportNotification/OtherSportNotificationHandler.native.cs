using MediatR;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Services;

namespace OWP.OtherSportNotification;

public class OtherSportNotificationHandler : IRequestHandler<OtherSportNotificationRequest, Unit>
{
    private readonly ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase;
    private readonly AssigneeChangedValueFilter assigneeChangedValueFilter;
    private readonly ChangedValueAggregator changedValueAggregator;
    private readonly MessageFactory messageFactory;
    private readonly OtherSportRepository repository;
    private readonly NotificationService notificationService;

    private readonly IncludeBBSectionService bBSectionService;

    public OtherSportNotificationHandler(ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
        AssigneeChangedValueFilter assigneeChangedValueFilter,
        ChangedValueAggregator changedValueAggregator,
        MessageFactory messageFactory,
        OtherSportRepository repository,
        NotificationService notificationService,
        IncludeBBSectionService bBSectionService)
    {
        this.changedValueByCacheAndDatabase = changedValueByCacheAndDatabase;
        this.assigneeChangedValueFilter = assigneeChangedValueFilter;
        this.changedValueAggregator = changedValueAggregator;
        this.messageFactory = messageFactory;
        this.repository = repository;
        this.notificationService = notificationService;
        this.bBSectionService = bBSectionService;
    }

    public async Task<Unit> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);

        await notificationService.Notify(messageModels);

        await bBSectionService.Execute(messageModels);
        await repository.InsertMessage(messageModels, request.SessionId, request.ActorId);


        return Unit.Value;
    }
}
