using Fanex.Caching;
using Newtonsoft.Json;
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
            if (startTime.HasValue)
            {
                return;
            }

            var leagueLatest = await repository.QueryDayShiftPreMatchAssignments(
                Helper.Helper.CreateShiftCriteria(requestDate, false, startTime.Value));

            await notificationServiceCommon.SendNotificationForNewLeague(leagueLatest, requestDate, true);
            
        }
    }
}
