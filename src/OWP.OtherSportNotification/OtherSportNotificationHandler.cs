using MediatR;
using OWP.Core.Contants;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Services;
using OWP.RabbitMQ.Publisher;

namespace OWP.OtherSportNotification;

public class OtherSportNotificationHandler : IRequestHandler<OtherSportNotificationRequest, Unit>
{
    private readonly ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase;
    private readonly AssigneeChangedValueFilter assigneeChangedValueFilter;
    private readonly ChangedValueAggregator changedValueAggregator;
    private readonly MessageFactory messageFactory;
    private readonly OtherSportRepository repository;
    private readonly MessagePublisher publisher;

    public OtherSportNotificationHandler(ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
        AssigneeChangedValueFilter assigneeChangedValueFilter,
        ChangedValueAggregator changedValueAggregator,
        MessageFactory messageFactory,
        OtherSportRepository repository,
        MessagePublisher publisher)
    {
        this.changedValueByCacheAndDatabase = changedValueByCacheAndDatabase;
        this.assigneeChangedValueFilter = assigneeChangedValueFilter;
        this.changedValueAggregator = changedValueAggregator;
        this.messageFactory = messageFactory;
        this.repository = repository;
        this.publisher = publisher;
    }

    public async Task<Unit> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate); var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues);

        await repository.InsertMessage(messageModels, request.SessionId, request.ActorId);

        publisher.PushMessage(messageModels, RoutingKey.OtherSport);

        return Unit.Value;
    }
}
