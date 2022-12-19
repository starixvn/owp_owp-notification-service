namespace OWP.AssignmentUploadNotification
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using OWP.Core.Models;
    using OWP.Core.NotificationRequests.AssignmentUpload;
    using OWP.Core.Services;
    using Services;
    using MessageCore = OWP.Core.Models;

    public class NotifyRequest : IRequest<bool>
    {
        public string SessionId { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsUploadFMAndAIO { get; set; }

        public string UserName { get; set; }

        public int ActorId { get; set; }
    }

    public class Handler : IRequestHandler<AssignmentUploadNotifyRequest, Unit>
    {
        private readonly ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase;
        private readonly AssigneeChangedValueFilter assigneeChangedValueFilter;
        private readonly WatcherChangedValueGenerator watcherChangedValueGenerator;
        private readonly ChangedValueAggregator changedValueAggregator;
        private readonly MessageFactory messageFactory;
        private readonly CommandRepository commandRepository;

        private readonly NotificationService notificationService;

        public Handler(
            ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
            AssigneeChangedValueFilter assigneeChangedValueFilter,
            WatcherChangedValueGenerator watcherChangedValueGenerator,
            ChangedValueAggregator changedValueAggregator,
            MessageFactory messageFactory,
            CommandRepository commandRepository,
            NotificationService notificationService)
        {
            this.changedValueByCacheAndDatabase = changedValueByCacheAndDatabase;
            this.assigneeChangedValueFilter = assigneeChangedValueFilter;
            this.watcherChangedValueGenerator = watcherChangedValueGenerator;
            this.changedValueAggregator = changedValueAggregator;
            this.messageFactory = messageFactory;
            this.commandRepository = commandRepository;
            this.notificationService = notificationService;
        }

        public async Task<Unit> Handle(AssignmentUploadNotifyRequest request, CancellationToken cancellationToken)
        {
            var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.EventDate);
            var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
            var watcherChangedValues = await watcherChangedValueGenerator.Create(messages, assigneeChangedValues);
            var allChangedValues = watcherChangedValues.Union(assigneeChangedValues).ToList();
            var aggregatedChangedValues = changedValueAggregator.Aggregate(allChangedValues);
            var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);
            await commandRepository.InsertMessage(messageModels, request.SessionId, request.ActorId);
            var messageSender = BuildMessageSender(messageModels);
            await notificationService.Notify(messageSender);

            return Unit.Value;
        }

        private IList<MessageModelCommon<MessageCore.MessageModel>> BuildMessageSender(IList<Models.MessageModel> messageModels)
        => messageModels.Select(m => new MessageModelCommon<MessageCore.MessageModel>()
        {
            Message = new()
            {
                MessageContent = m.MessageContent,
                MessageType = m.MessageType,
                IsImportant = m.IsImportant
            },
            ReceivedUserIds = m.ReceivedUserIds
        }).ToList();
    }
}
