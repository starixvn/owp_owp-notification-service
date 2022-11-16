namespace OWP.AssignmentUploadNotification
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using OWP.AssignmentUploadNotification.Models;
    using OWP.Core.Models;
    using OWP.Core.Services;
    using Services;

    public class NotifyRequest : IRequest<bool>
    {
        public string SessionId { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsUploadFMAndAIO { get; set; }
    }

    public class AssignmentUploadHandler : IRequestHandler<NotifyRequest, bool>
    {
        private readonly ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase;
        private readonly AssigneeChangedValueFilter assigneeChangedValueFilter;
        private readonly WatcherChangedValueGenerator watcherChangedValueGenerator;
        private readonly ChangedValueAggregator changedValueAggregator;
        private readonly MessageFactory messageFactory;
        private readonly CommandRepository commandRepository;

         private readonly NotificationService notificationService;
         
        public AssignmentUploadHandler(
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

        public async Task<bool> Handle(NotifyRequest request, CancellationToken cancellationToken)
        {
            var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.EventDate);
            var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
            var watcherChangedValues = await watcherChangedValueGenerator.Create(messages, assigneeChangedValues);
            var allChangedValues = assigneeChangedValues.Union(watcherChangedValues).ToList();

            var aggregatedChangedValues = changedValueAggregator.Aggregate(allChangedValues);
            var messageModels = messageFactory.Transform(aggregatedChangedValues);

            await commandRepository.InsertMessage(messageModels, request.SessionId);

            var messageSender= BuildMessageSenderInfo(messageModels);
            
            await notificationService.Notify(messageSender);

            return await Task.FromResult(true);
        }

        private IList<MessageModelCommon<MessageInfo>>BuildMessageSenderInfo(IList<MessageInfo> messageModels)
        => messageModels
                .Select(m=>new MessageModelCommon<MessageInfo>{
                    Message= m,
                    ReceivedUserIds= m.ReceivedUserIdList.Split(",")
                }).ToList();
    }
}
