using System.Linq.Expressions;
using MediatR;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services
{
    public interface RocketChatNotifyEventUnlink
    {
        Task SendMessageUnlinkEvent(HttpClient client, OtherSportNotificationRequest request);
    }

    public class RocketChatNotifyEventUnlinkImp(OtherSportRepository repository,
        CacheManipulator cacheManipulator,
        RocketChatMessageToChannel messageToChannel) : RocketChatNotifyEventUnlink
    {
        int BATCH_SIZE = 10;
        private const string ChannelName = "#LC_Update_Notification";
        public async Task SendMessageUnlinkEvent(HttpClient client, OtherSportNotificationRequest request)
        {
            var newSnapshot = await repository.Query(request.FromDate, request.ToDate);
            var matches = newSnapshot.Matches.Values;
            var newMatchIds = matches
                .Where(x => x.OfferLive == "Live" && x.AdminsiteMatchId is null)
                .Select(x => x.OSMatchId)
                .Distinct()
                .ToList();

            var eventDates = new List<DateTime>();
            var diffDays = request.FromDate - request.ToDate;

            for (var i = 0; i <= diffDays.TotalDays; i++)
            {
                eventDates.Add(request.FromDate.AddDays(i));
            }

            foreach (var eventDate in eventDates)
            {
                await ProcessUnlinkNotificationForDate(client, eventDate, newMatchIds, matches.ToList(), request);
            }
        }

        private async Task ProcessUnlinkNotificationForDate(
            HttpClient client, DateTime eventDate, List<int> newMatchIds, List<MatchInfo> assignments
            , OtherSportNotificationRequest request)
        {
            var previouslyNotifiedMatches = await cacheManipulator.GetNotificatedMatchesUnlink(eventDate);
            var matchesToUnlink = newMatchIds.Except(previouslyNotifiedMatches).ToList();

            await SendUnlinkNotification(client, matchesToUnlink, assignments, request, eventDate);

        }

        private async Task SendUnlinkNotification(HttpClient client, List<int> matchesToUnlink, List<MatchInfo> assignments, OtherSportNotificationRequest request, DateTime eventDate)
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

            // TODO
           await SendNotification(client, unlinkNotificationList, request, eventDate);
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

        private async Task SendNotification(HttpClient client, List<UnlinkNotificationList> messagesList,OtherSportNotificationRequest request, DateTime eventDate)
        {
            for (var i = 0; i < messagesList.Count; i += BATCH_SIZE)
            {
                var batch = messagesList.Skip(i).Take(BATCH_SIZE).ToList();
                var messages = batch.Select(x => x.Message).ToList();

                await SendBatch(client, messages, request);
                await cacheManipulator.AddMoreMatchIdsToCache(eventDate, batch.Select(x => x.OsMatchId).ToList());
            }
        }

        private async Task SendBatch(HttpClient client, List<string> batch, OtherSportNotificationRequest request)
        {
            var message = new
            {
                channel = ChannelName,
                text = string.Join("\n-------------------\n", batch)
            };
            await messageToChannel.MessageToChannel(client, message, request);
        }
    }
}
