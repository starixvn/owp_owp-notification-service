using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Services;

namespace OWP.OtherSportNotification;

public class OtherSportNotificationHandler : IRequestHandler<OtherSportNotificationRequest, string>
{
    private readonly ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase;
    private readonly AssigneeChangedValueFilter assigneeChangedValueFilter;
    private readonly ChangedValueAggregator changedValueAggregator;
    private readonly MessageFactory messageFactory;
    private readonly OtherSportRepository repository;

    public OtherSportNotificationHandler(ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
        AssigneeChangedValueFilter assigneeChangedValueFilter,
        ChangedValueAggregator changedValueAggregator,
        MessageFactory messageFactory,
        OtherSportRepository repository)
    {
        this.changedValueByCacheAndDatabase = changedValueByCacheAndDatabase;
        this.assigneeChangedValueFilter = assigneeChangedValueFilter;
        this.changedValueAggregator = changedValueAggregator;
        this.messageFactory = messageFactory;
        this.repository = repository;
    }

    public async Task<string> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues);

        await repository.InsertMessage(messageModels, request.SessionId, request.ActorId);

        var result = messageModels.Select(m => new
        {
            NotificationId = Guid.NewGuid().ToString(),
            Message = m
        }).ToList();

        return JsonConvert.SerializeObject(result);
    }
}
