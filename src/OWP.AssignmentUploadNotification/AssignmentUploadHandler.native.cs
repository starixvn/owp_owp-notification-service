namespace OWP.AssignmentUploadNotification;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OWP.AssignmentUploadNotification.Models;
using OWP.Core.Models;
using OWP.Core.NotificationRequests.AssignmentUpload;
using OWP.Core.Services;
using Services;
using Fanex.Logging;
using Newtonsoft.Json;

public class AssignmentUploadHandler : IRequestHandler<AssignmentUploadNotifyRequest, Unit>
{
    private readonly ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase;
    private readonly AssigneeChangedValueFilter assigneeChangedValueFilter;
    private readonly WatcherChangedValueGenerator watcherChangedValueGenerator;
    private readonly ChangedValueAggregator changedValueAggregator;
    private readonly MessageFactory messageFactory;
    private readonly CommandRepository commandRepository;

    private readonly NotificationService notificationService;

    private readonly ILogger _logger;

    public AssignmentUploadHandler(
        ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
        AssigneeChangedValueFilter assigneeChangedValueFilter,
        WatcherChangedValueGenerator watcherChangedValueGenerator,
        ChangedValueAggregator changedValueAggregator,
        MessageFactory messageFactory,
        CommandRepository commandRepository,
        NotificationService notificationService,
        ILogger logger)
    {
        this.changedValueByCacheAndDatabase = changedValueByCacheAndDatabase;
        this.assigneeChangedValueFilter = assigneeChangedValueFilter;
        this.watcherChangedValueGenerator = watcherChangedValueGenerator;
        this.changedValueAggregator = changedValueAggregator;
        this.messageFactory = messageFactory;
        this.commandRepository = commandRepository;
        this.notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Unit> Handle(AssignmentUploadNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.EventDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var watcherChangedValues = await watcherChangedValueGenerator.Create(messages, assigneeChangedValues);
        var allChangedValues = assigneeChangedValues.Union(watcherChangedValues).ToList();

        var aggregatedChangedValues = changedValueAggregator.Aggregate(allChangedValues);
        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);

        await commandRepository.InsertMessage(messageModels, request.SessionId, request.ActorId);

        var messageSender = BuildMessageSenderInfo(messageModels);
        await notificationService.Notify(messageSender);

        return Unit.Value;
    }

    private IList<MessageModelCommon<Models.MessageModel>> BuildMessageSenderInfo(IList<Models.MessageModel> messageModels)
    => messageModels
            .Select(m => new MessageModelCommon<Models.MessageModel>
            {
                Message = m,
                ReceivedUserIds = m.ReceivedUserIdList.Split(",")
            }).ToList();
}
