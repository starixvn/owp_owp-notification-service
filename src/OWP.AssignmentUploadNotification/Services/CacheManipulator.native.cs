namespace OWP.AssignmentUploadNotification.Services;

using Fanex.Caching;
using Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

public interface CacheManipulator
{
    Task<AssignmentSnapshot> GetAssignmentSnapshot(string sessionId);
}

public class CacheManipulatorImp(ICache cache) : CacheManipulator
{
    private const string KEY_ASSIGNMENT_SNAPSHOT = "OWP-ASSIGNMENT-SNAPSHOT";

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