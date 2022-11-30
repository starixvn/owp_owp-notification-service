namespace OWP.AssignmentUploadNotification
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MediatR;
    using Newtonsoft.Json;
    using OWP.AssignmentUploadNotification.Models;
    using OWP.Core.NotificationRequests.AssignmentUpload;
    using Services;

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

        public Handler(
            ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
            AssigneeChangedValueFilter assigneeChangedValueFilter,
            WatcherChangedValueGenerator watcherChangedValueGenerator,
            ChangedValueAggregator changedValueAggregator,
            MessageFactory messageFactory,
            CommandRepository commandRepository)
        {
            this.changedValueByCacheAndDatabase = changedValueByCacheAndDatabase;
            this.assigneeChangedValueFilter = assigneeChangedValueFilter;
            this.watcherChangedValueGenerator = watcherChangedValueGenerator;
            this.changedValueAggregator = changedValueAggregator;
            this.messageFactory = messageFactory;
            this.commandRepository = commandRepository;
        }

        public async Task<Unit> Handle(AssignmentUploadNotifyRequest request, CancellationToken cancellationToken)
        {
            var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.EventDate);

            var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
            if (assigneeChangedValues?.Count > 0)
            {
                LogService.Info("assigneeChangedValues1" + JsonConvert.SerializeObject(assigneeChangedValues));
            }
            var watcherChangedValues = await watcherChangedValueGenerator.Create(messages, assigneeChangedValues);
            if (watcherChangedValues?.Count > 0)
            {
                LogService.Info("watcherChangedValues" + JsonConvert.SerializeObject(watcherChangedValues));
            }
            var allChangedValues = watcherChangedValues.Union(assigneeChangedValues).ToList();
            LogService.Info("assigneeChangedValues2" + JsonConvert.SerializeObject(assigneeChangedValues));
            if (allChangedValues?.Count > 0)
            {
                LogService.Info("allChangedValues" + JsonConvert.SerializeObject(allChangedValues));
            }
            var aggregatedChangedValues = changedValueAggregator.Aggregate(allChangedValues);
            if (aggregatedChangedValues?.Count > 0)
            {
                LogService.Info("aggregatedChangedValues" + JsonConvert.SerializeObject(aggregatedChangedValues));
            }
            var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);
            if (messageModels?.Count > 0)
            {
                LogService.Info("messageModels" + JsonConvert.SerializeObject(messageModels));
            }
            await commandRepository.InsertMessage(messageModels, request.SessionId, request.ActorId);

            return Unit.Value;
        }
    }
}
