using Fanex.Caching;
using Newtonsoft.Json;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface CacheManipulator
{
    Task DeleteAssignmentSnapshot(string sessionId);
    Task<AssignmentSnapshot> GetAssignmentSnapshot(string sessionId);
    Task SaveNotificatedMatchesUnlink(DateTime eventDate, List<int> osMatchIds);
    Task<List<int>> GetNotificatedMatchesUnlink(DateTime eventDate);
    Task AddMoreMatchIdsToCache(DateTime eventDate, List<int> additionalMatchIds);
}

public class CacheManipulatorImp(ICache cache) : CacheManipulator
{
    private const string KEY_OLD_OS_ASSIGNMENT_SNAPSHOT = "OWP-Old-OS-Assignment";
    private const string KEY_OS_ASSIGNMENT = "OWP-OS-AssignmentUpload";
    private const int CACHE_OS_MATCHES_UNLINK_NOTIFICATED_EXPIRED_IN_DAY = 5;
    private const string KEY_OS_MATCHES_UNLINK_NOTIFICATED = "OWP-OS-Matches-Unlink-Notificated";

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

    public async Task<List<AssignmentSnapshot>> GetNewAssignments(string sessionId)
    {
        var cacheKey = $"{KEY_OS_ASSIGNMENT}-{sessionId}";
        var (_, value) = await cache.TryGetAsync<string>(cacheKey);

        var assignments = string.IsNullOrEmpty(value)
            ? []
            : JsonConvert.DeserializeObject<List<AssignmentSnapshot>>(value);

        return assignments ?? [];
    }

    public async Task DeleteAssignments(string sessionId)
    {
        var cacheKey = $"{KEY_OS_ASSIGNMENT}-{sessionId}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task SaveNotificatedMatchesUnlink(DateTime eventDate, List<int> osMatchIds)
    {
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_OS_MATCHES_UNLINK_NOTIFICATED_EXPIRED_IN_DAY));
        var cacheKey = $"{KEY_OS_MATCHES_UNLINK_NOTIFICATED}-{eventDate}";
        await cache.SetAsync(cacheKey, osMatchIds, cachePolicy);
    }

    public async Task<List<int>> GetNotificatedMatchesUnlink(DateTime eventDate)
    {
        var cacheKey = $"{KEY_OS_MATCHES_UNLINK_NOTIFICATED}-{eventDate}";
        var (_, value) = await cache.TryGetAsync<List<int>>(cacheKey);

        return value ?? [];
    }

    public async Task AddMoreMatchIdsToCache(DateTime eventDate, List<int> additionalMatchIds)
    {
        var existingMatchIds = await GetNotificatedMatchesUnlink(eventDate);
        var updatedMatchIds = existingMatchIds.Union(additionalMatchIds).ToList();
        await SaveNotificatedMatchesUnlink(eventDate, updatedMatchIds);
    }
}
