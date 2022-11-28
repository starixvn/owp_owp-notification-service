namespace OWP.AssignmentUploadNotification.Services
{
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Newtonsoft.Json;
    using OWP.AssignmentUploadNotification.Models;
    using Fanex.Logging;
    public interface CacheManipulator
    {
        Task<AssignmentSnapshot> GetAssignmentSnapshot(string sessionId);
    }

    public class CacheManipulatorImp : CacheManipulator
    {
        private readonly ICache cache;
        private readonly ILogger logger;
        private const string KEY_ASSIGNMENT_SNAPSHOT = "OWP-ASSIGNMENT-SNAPSHOT";

        public CacheManipulatorImp(ICache cache, ILogger logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<AssignmentSnapshot> GetAssignmentSnapshot(string sessionId)
        {
            var cacheKey = $"{KEY_ASSIGNMENT_SNAPSHOT}-{sessionId}";
            var cacheResult = await cache.TryGetAsync<string>(cacheKey);
            var assignmentSnapshot = string.IsNullOrEmpty(cacheResult.Item2)
                ? new AssignmentSnapshot()
                : JsonConvert.DeserializeObject<AssignmentSnapshot>(cacheResult.Item2);
            return assignmentSnapshot ?? new AssignmentSnapshot();
        }
    }
}
