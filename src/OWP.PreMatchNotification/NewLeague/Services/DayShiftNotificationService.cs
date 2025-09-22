using OWP.PreMatchNotification.Shared.Services;


namespace OWP.PreMatchNotification.NewLeague.Services
{
    public interface DayShiftNotificationService
    {
        Task SendNotificationForNewLeague();
    }

    public class DayShiftNotificationServiceImp(
        NotificationServiceCommon notificationServiceCommon,
    CommandRepository repository) : DayShiftNotificationService
    {
        
        public async Task SendNotificationForNewLeague()
        {
            var requestDate = DateTimeOffset.Now.AddHours(12).Date;

            var (leagueLatest, infoSiteMatches) = await repository.QueryDayShiftPreMatchAssignments(
                Helper.Helper.CreateShiftCriteria(requestDate, true));
            var leagueExcludeLeagueTest = Helper.Helper.ExcludeLeagueTest(leagueLatest, infoSiteMatches);
            await notificationServiceCommon.SendNotificationForNewLeague(leagueExcludeLeagueTest, requestDate, true);
        }

    }
}
