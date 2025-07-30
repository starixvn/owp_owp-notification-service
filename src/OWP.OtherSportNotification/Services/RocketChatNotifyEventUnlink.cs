using System.Threading.Tasks;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services
{
    public interface RocketChatNotifyEventUnlink
    {
        Task SendMessageUnlinkEvent(string sessionId, DateTime fromDate, DateTime toDate);
    }

    public class RocketChatNotifyEventUnlinkImp(OtherSportRepository repository,
        CacheManipulator cacheManipulator) : RocketChatNotifyEventUnlink
    {
        public async Task SendMessageUnlinkEvent(string sessionId,DateTime fromDate,DateTime toDate)
        {
            var newSnapshot = await repository.Query(fromDate, toDate);
            var matches = newSnapshot.Matches.Values;
            var newMatchIds = matches
                .Where(x => x.OfferLive == "Live" && x.AdminsiteMatchId == null)
                .Select(x => x.OSMatchId)
                .Distinct()
                .ToList();

            var eventDates = new List<DateTime>();
            var diffDays = toDate - fromDate;

            for (var i = 0; i <= diffDays.TotalDays; i++)
            {
                eventDates.Add(fromDate.AddDays(i));
            }

            foreach (var eventDate in eventDates)
            {
                await ProcessUnlinkNotificationForDate(eventDate, newMatchIds, matches.ToList());
            }
        }

        private async Task ProcessUnlinkNotificationForDate(DateTime eventDate, List<int> newMatchIds, List<MatchInfo> assignments)
        {
            var previouslyNotifiedMatches = await cacheManipulator.GetNotificatedMatchesUnlink(eventDate);
            var matchesToUnlink = newMatchIds.Except(previouslyNotifiedMatches).ToList();

            await SendUnlinkNotification(eventDate, matchesToUnlink, assignments);

        }


        private async Task SendUnlinkNotification(DateTime eventDate, List<int> matchesToUnlink, List<MatchInfo> assignments)
        {
            var assignmentsToNotify = assignments.Where(x => matchesToUnlink.Contains(x.OSMatchId)).ToList();

            var sortedAssignments = assignmentsToNotify
                .OrderBy(x => GetSportPriority(x.Sport))
                .ThenBy(x => x.DateTime)
                .ToList();

            var unlinkNotificationList = new List<UnlinkNotificationList>();

            foreach (var assignment in sortedAssignments)
            {
                var notificationMessage = CreateMatchNotificationMessage(assignment);
                unlinkNotificationList.Add(new UnlinkNotificationList
                {
                    OsMatchId = assignment.OSMatchId,
                    Message = notificationMessage
                });
            }

            await SendNotification(unlinkNotificationList, eventDate);
        }

        private int GetSportPriority(string sport)
        {
            return sport?.ToLower() switch
            {
                "basketball" => 1,
                "tennis" => 2,
                _ => 3 
            };
        }

        private string CreateMatchNotificationMessage(MatchInfo assignment)
        {
            return $"{assignment.Sport}\nK.O Time: {assignment.DateTime.ToString("yyyy-MM-dd HH:mm")}\nLeague: {assignment.League}\nMatch: {assignment.Home} - {assignment.Away}";
        }

        private async Task SendNotification(List<UnlinkNotificationList> unlinkNotificationList, DateTime eventDate)
        {
            foreach (var notification in unlinkNotificationList)
            {
                Console.WriteLine($"\n{notification.Message}\n--------------------------------");

                await cacheManipulator.AddMoreMatchIdsToCache(eventDate, [notification.OsMatchId]);
            }
        }
    }
}
