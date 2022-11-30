namespace OWP.AssignmentUploadNotification.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Newtonsoft.Json;
    using OWP.AssignmentUploadNotification.Models;

    public interface CacheManipulator
    {
        Task<AssignmentSnapshot> GetAssignmentSnapshot(string sessionId);
    }

    public class CacheManipulatorImp : CacheManipulator
    {
        private readonly ICache cache;
        private const string KEY_ASSIGNMENT_SNAPSHOT = "OWP-ASSIGNMENT-SNAPSHOT";

        public CacheManipulatorImp(ICache cache)
        {
            this.cache = cache;
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
