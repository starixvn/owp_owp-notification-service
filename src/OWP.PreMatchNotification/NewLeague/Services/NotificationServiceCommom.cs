using Newtonsoft.Json;
using OWP.PreMatchNotification.Shared.Services;


namespace OWP.PreMatchNotification.NewLeague.Services
{
    public interface NotificationServiceCommon
    {
        Task SendNotificationForNewLeague(IList<string> leagueLatest, DateTime requestDate, bool isDay);
    }

    public class NotificationServiceCommonImp(
        CacheManipulatorCommon cacheManipulator) : NotificationServiceCommon
    {
        private const string PREMATCH_SHIFT = "prematch-shift";
        private const int CACHE_EXPIRED_IN_3_DAYS = 3;

        public async Task SendNotificationForNewLeague(IList<string> leagueLatest, DateTime requestDate, bool isDay)
        {
            var cacheKey = BuildKey(requestDate, isDay);
            var leagueInCache = await cacheManipulator.GetMultipleData<string>(cacheKey);

            var newLeague = !leagueInCache.Any() 
                ? [] 
                : leagueLatest.Except(leagueInCache, StringComparer.OrdinalIgnoreCase).ToList();
            // TODO
            // Msg to RC for new league

            if (cacheManipulator.ContainKey(cacheKey))
            {
                await cacheManipulator.RemoveData(cacheKey);
            }
            var assignmentSnapshotInJson = JsonConvert.SerializeObject(leagueLatest.Union(leagueInCache));
            await cacheManipulator.Cache(assignmentSnapshotInJson, cacheKey, TimeSpan.FromMinutes(CACHE_EXPIRED_IN_3_DAYS));
        }

        private static string BuildKey(DateTime requestDate, bool isDay)
            => isDay
                ? $"{PREMATCH_SHIFT}-{requestDate:yyyy-MM-dd}-day-shift"
                : $"{PREMATCH_SHIFT}-{requestDate:yyyy-MM-dd}-night-shift";
    }
}
