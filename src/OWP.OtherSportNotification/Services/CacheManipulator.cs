using Fanex.Caching;
using Newtonsoft.Json;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface CacheManipulator
{
    Task DeleteAssignmentSnapshot(string sessionId);
    Task<AssignmentSnapshot> GetAssignmentSnapshot(string sessionId);
}

public class CacheManipulatorImp : CacheManipulator
{
    private readonly ICache cache;
    private const string KEY_OLD_OS_ASSIGNMENT_SNAPSHOT = "OWP-Old-OS-Assignment";

    public CacheManipulatorImp(ICache cache)
    {
        this.cache = cache;
    }

    public async Task<AssignmentSnapshot> GetAssignmentSnapshot(string sessionId)
    {
        var cacheKey = $"{KEY_OLD_OS_ASSIGNMENT_SNAPSHOT}-{sessionId}";
        var cacheResult = await cache.TryGetAsync<string>(cacheKey);
        var assignmentSnapshot = string.IsNullOrEmpty(cacheResult.Item2)
            ? new AssignmentSnapshot()
            : JsonConvert.DeserializeObject<AssignmentSnapshot>(cacheResult.Item2);
        return assignmentSnapshot ?? new AssignmentSnapshot();
    }

    public async Task DeleteAssignmentSnapshot(string sessionId)
    {
        var cacheKey = $"{KEY_OLD_OS_ASSIGNMENT_SNAPSHOT}-{sessionId}";
        await cache.RemoveAsync(cacheKey);
    }
}
