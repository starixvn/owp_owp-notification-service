using Fanex.Caching;
using Newtonsoft.Json;
using OWP.PreMatchNotification.Shared.Services;


namespace OWP.PreMatchNotification.NewLeague.Services
{
    public interface DayShiftNotificationService
    {
        Task SendNotificationForNewLeague();
    }

    public class DayShiftNotificationServiceImp(
        CacheManipulatorCommon cacheManipulator,
    CommandRepository repository) : DayShiftNotificationService
    {
        private const string PREMATCH_SHIFT = "prematch-shift";
        private const int CACHE_EXPIRED_IN_3_DAYS = 3;

        public async Task SendNotificationForNewLeague()
        {
            var requestDate = DateTimeOffset.Now.AddHours(12).Date;

            var leagueLatest = await repository.QueryDayShiftPreMatchAssignments(
                Helper.Helper.CreateShiftCriteria(requestDate, true));

            var cacheKey = BuildKey(requestDate);
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

        private static string BuildKey(DateTime requestDate)
            => $"{PREMATCH_SHIFT}-{requestDate:yyyy-MM-dd}-day-shift";
    }
}
