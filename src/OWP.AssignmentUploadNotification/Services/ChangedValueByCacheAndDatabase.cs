namespace OWP.AssignmentUploadNotification.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Models;
    using Newtonsoft.Json;

    public interface ChangedValueByCacheAndDatabase
    {
        Task<List<ChangedValue>> Create(string sessionId, DateTime eventDate);
    }

    public class ChangedValueCacheAndDatabaseImp : ChangedValueByCacheAndDatabase
    {
        private readonly CacheManipulator cacheManipulator;
        private readonly QueryRepository queryRepository;
        private readonly ChangedValueBySnapshot changedValueBySnapshot;

        private readonly ILogger _logger;

        public ChangedValueCacheAndDatabaseImp(
            CacheManipulator cacheManipulator,
            QueryRepository queryRepository,
            ChangedValueBySnapshot changedValueBySnapshot,
            ILogger logger)
        {
            this.cacheManipulator = cacheManipulator;
            this.queryRepository = queryRepository;
            this.changedValueBySnapshot = changedValueBySnapshot;
            _logger = logger;
        }

        public async Task<List<ChangedValue>> Create(string sessionId, DateTime eventDate)
        {
            var oldSnapshot = await cacheManipulator.GetAssignmentSnapshot(sessionId);
            var newSnapshot = await queryRepository.Query(eventDate);

            _logger.Info(JsonConvert.SerializeObject(oldSnapshot));
            _logger.Info(JsonConvert.SerializeObject(newSnapshot));
            var changedValue = changedValueBySnapshot.Create(oldSnapshot, newSnapshot);
            if (changedValue?.Count > 0)
            {
                _logger.Info("changedValue" + JsonConvert.SerializeObject(changedValue));
            }
            return changedValue;
        }
    }
}
