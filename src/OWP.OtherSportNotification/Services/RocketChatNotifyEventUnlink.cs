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
        public async Task SendMessageUnlinkEvent(
        string sessionId,
        DateTime fromDate,
        DateTime toDate)
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

            SendUnlinkNotification(eventDate, matchesToUnlink, assignments);

            // Combine previously notified matches with new matches to unlink
            var updatedNotifiedMatches = previouslyNotifiedMatches.Union(matchesToUnlink).ToList();
            if (updatedNotifiedMatches.Count != previouslyNotifiedMatches.Count)
            {
                await cacheManipulator.SaveNotificatedMatchesUnlink(eventDate, updatedNotifiedMatches);
            }
        }


        private void SendUnlinkNotification(DateTime eventDate, List<int> matchesToUnlink, List<MatchInfo> assignments)
        {
            var assignmentsToNotify = assignments.Where(x => matchesToUnlink.Contains(x.OSMatchId)).ToList();

            // Sort by sport (Basketball, Tennis, Other Sport) and then by earliest kick-off time
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

            SendNotification(unlinkNotificationList);
        }

        private int GetSportPriority(string sport)
        {
            return sport?.ToLower() switch
            {
                "basketball" => 1,
                "tennis" => 2,
                _ => 3 // Other Sport
            };
        }

        private string CreateMatchNotificationMessage(MatchInfo assignment)
        {
            return $"{assignment.Sport}\nK.O Time: {assignment.DateTime.ToString("yyyy-MM-dd HH:mm")}\nLeague: {assignment.League}\nMatch: {assignment.Home} - {assignment.Away}";
        }

        private void SendNotification(List<UnlinkNotificationList> unlinkNotificationList)
        {
            foreach (var notification in unlinkNotificationList)
            {
                Console.WriteLine($"\n{notification.Message}\n--------------------------------");
            }
        }
    }
}
