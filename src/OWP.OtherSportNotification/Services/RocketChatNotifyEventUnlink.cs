using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using Fanex.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OWP.OtherSportNotification.Services
{
    public interface RocketChatNotifyEventUnlink
    {
        Task SendMessageUnlinkEvent(HttpClient client, OtherSportNotificationRequest request);
    }

    public class RocketChatNotifyEventUnlinkImp(OtherSportRepository repository,
        CacheManipulator cacheManipulator,
        RocketChatMessageToChannel messageToChannel,
        ILogger logger) : RocketChatNotifyEventUnlink
    {
        const int BATCH_SIZE = 10;
        private const string ChannelName = "#MissingGameNotify";
        public async Task SendMessageUnlinkEvent(HttpClient client, OtherSportNotificationRequest request)
        {
            await Task.Delay(TimeSpan.FromSeconds(180));
            var eventsUnlinkInfo = await GetEventsUnlink(request);
            var (fromDate, toDate) = NormalizeTimeRange(request.FromDate, request.ToDate);
            var (basketballGroup, tennisGroup, otherGroup) = CategoryEventBySportType(eventsUnlinkInfo);
            var eventsWillSendNotyBasketballGroup = await BuildEventWillSendNoty(fromDate, toDate, basketballGroup);
            var eventsWillSendNotyTennisGroup = await BuildEventWillSendNoty(fromDate, toDate, tennisGroup);
            var eventsWillSendNotyOtherGroup = await BuildEventWillSendNoty(fromDate, toDate, otherGroup);

            var allEvents = eventsWillSendNotyBasketballGroup
                .Concat(eventsWillSendNotyTennisGroup)
                .Concat(eventsWillSendNotyOtherGroup)
                .ToList();

            var osIdAlreadySendNoty = await SendMessagesInBatches(client, allEvents, request);
            await CacheEventAlreadySendNoty(fromDate, toDate, osIdAlreadySendNoty, allEvents);
        }

        
        private async Task<IList<MatchInfo>> BuildEventWillSendNoty(DateTime fromDate, DateTime toDate, IList<MatchInfo> groupEvents)
        {
            var listEventWillSendNoty = new List<MatchInfo>();
            for (var day = fromDate; day <= toDate; day = day.AddDays(1))
            {
                var eventsUnlinkInDay = GetEventsInDay(day, groupEvents);
                var eventWillSendNoty= await RemoveEventAlreadySendNotyInDay(eventsUnlinkInDay, day);
                listEventWillSendNoty.AddRange(eventWillSendNoty);
            }
            return listEventWillSendNoty;


        }

        private async Task CacheEventAlreadySendNoty(DateTime fromDate, DateTime toDate, IList<int>osIdAlreadySendNoty, IList<MatchInfo> eventsUnlink)
        {
            var eventsAlreadySendNoty = eventsUnlink.Where(e => osIdAlreadySendNoty.Contains(e.OSMatchId)).ToList();
            for (var day = fromDate; day <= toDate; day = day.AddDays(1))
            {
                var osIdAlreadySendNotyInDay = 
                    GetEventsInDay(day, eventsAlreadySendNoty)
                        .Select(e=>e.OSMatchId)
                        .ToList();
                await cacheManipulator.AddMoreMatchIdsToCache(day, osIdAlreadySendNotyInDay);
            }
        }

        private async Task<IList<MatchInfo>> RemoveEventAlreadySendNotyInDay(IList<MatchInfo> events, DateTime date)
        {
            var osMatchIdAlreadySendNoty = await cacheManipulator.GetOsMatchIdAlreadySendNoty(date);

            return events.Where(e => !osMatchIdAlreadySendNoty.Contains(e.OSMatchId)).ToList();
        }

        private (IList<MatchInfo> basketballGroup, IList<MatchInfo> tennisGroup, IList<MatchInfo> otherGroup) CategoryEventBySportType(IList<MatchInfo> eventsUnlinkInfo)
        {
            var basketballGroup = CategoryEventBySportType(eventsUnlinkInfo, sport=>sport.Equals("Basketball", StringComparison.OrdinalIgnoreCase)) ;
                
            var tennisGroup = CategoryEventBySportType(eventsUnlinkInfo, sport => sport.Equals("Tennis", StringComparison.OrdinalIgnoreCase));

            var otherGroup = eventsUnlinkInfo
                .Where(x => !x.Sport.Equals("Basketball", StringComparison.OrdinalIgnoreCase) &&
                           !x.Sport.Equals("Tennis", StringComparison.OrdinalIgnoreCase))
                .GroupBy(x => x.Sport)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .SelectMany(g => g.OrderBy(x => x.DateTime))
                .ToList();

            return (basketballGroup, tennisGroup, otherGroup);
        }

        private IList<MatchInfo> CategoryEventBySportType(IList<MatchInfo> events, Func<string,bool> predicate)
        {
            return events
                .Where(x => predicate(x.Sport))
                .OrderBy(x => x.DateTime)
                .ToList();
        }

        private IList<MatchInfo> GetEventsInDay(DateTime targetDay, IList<MatchInfo> eventsUnlinkInfo)
        {
            var (dayStart, dayEnd) = NormalizeTimeRange(targetDay, targetDay);
            var eventsUnlinkInDay = eventsUnlinkInfo.Where(
                e => e.DateTime >= dayStart && e.DateTime <= dayEnd);
            return eventsUnlinkInDay.ToList();
        }

        private (DateTime fromDate, DateTime toDate) NormalizeTimeRange(DateTime dayStart, DateTime dayEnd)
        => (dayStart.Date.AddHours(12), dayEnd.Date.AddDays(1).AddHours(11).AddMinutes(59));

        private async Task<IList<MatchInfo>> GetEventsUnlink(OtherSportNotificationRequest request)
        {
            var newSnapshot = await repository.Query(request.FromDate, request.ToDate);
            var matches = newSnapshot.Matches.Values;
            return matches
                .Where(x => x.OfferLive == "Live" && x.AdminsiteMatchId is null)
                .ToList();
        }

        private async Task<List<int>> SendMessagesInBatches(HttpClient client, IList<MatchInfo> events, OtherSportNotificationRequest request)
        {
            var messageSendSuccessfully = new List<int>();
            for (var i = 0; i < events.Count; i += BATCH_SIZE)
            {
                var batches = events.Skip(i).Take(BATCH_SIZE).ToList();
                var messages = batches.Select(e => $"**{e.Sport}**\nK.O Time: {e.DateTime:yyyy-MM-dd HH:mm}\nLeague: {e.League}\nMatch: {e.Home} - {e.Away}");
                try
                {
                    var message = new
                    {
                        channel = ChannelName,
                        text = string.Join("\n-------------------\n", messages)
                    };
                    await messageToChannel.MessageToChannel(client, message, request);
                    messageSendSuccessfully.AddRange(batches.Select(m => m.OSMatchId));
                }
                catch (Exception ex)
                {
                    await logger.ErrorAsync($"Failed to send unlink notification messages in batches:{JsonConvert.SerializeObject(ex)}", "OsUnlinkMatch_MsgToRc");
                }
            }

            return messageSendSuccessfully;
        }
    }
}
