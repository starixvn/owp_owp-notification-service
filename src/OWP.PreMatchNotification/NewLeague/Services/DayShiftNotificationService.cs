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
        NotificationServiceCommon notificationServiceCommon,
    CommandRepository repository) : DayShiftNotificationService
    {
        
        public async Task SendNotificationForNewLeague()
        {
            var requestDate = DateTimeOffset.Now.AddHours(12).Date;

            var leagueLatest = await repository.QueryDayShiftPreMatchAssignments(
                Helper.Helper.CreateShiftCriteria(requestDate, true));

            await notificationServiceCommon.SendNotificationForNewLeague(leagueLatest, requestDate, true);
        }

    }
}
