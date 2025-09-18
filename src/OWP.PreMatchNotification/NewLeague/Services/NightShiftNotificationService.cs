using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.NewLeague.Services
{
    public interface NightShiftNotificationService
    {
        Task SendNotificationForNewLeague();
    }

    public class NightShiftNotificationServiceImp(
    CommandRepository repository,
    NotificationServiceCommon notificationServiceCommon) : NightShiftNotificationService
    {
        public async Task SendNotificationForNewLeague()
        {
            var requestDate = DateTimeOffset.Now.AddHours(12).Date;
            var startTime = await repository.QueryExistedStartTime(requestDate);
            if (!startTime.HasValue)
            {
                return;
            }
            
            var (leagueLatest, infoSiteMatches) = await repository.QueryDayShiftPreMatchAssignments(
                Helper.Helper.CreateShiftCriteria(requestDate, false, startTime.Value));
            var leagueExcludeLeagueTest = Helper.Helper.ExcludeLeagueTest(leagueLatest, infoSiteMatches);

            await notificationServiceCommon.SendNotificationForNewLeague(leagueExcludeLeagueTest, requestDate, false);
            
        }
    }
}
