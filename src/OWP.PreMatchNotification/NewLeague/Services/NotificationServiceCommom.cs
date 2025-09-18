using System.Text;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.PreMatchNotification.Shared.Services;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using OWP.Core.Exceptions;
using Olympus.OWP.Cache;
using OWP.Core.Services;
using OWP.Core.Utils;
using OWP.PreMatchNotification.NewLeague.Models;

namespace OWP.PreMatchNotification.NewLeague.Services
{
    public interface NotificationServiceCommon
    {
        Task SendNotificationForNewLeague(IList<string> leagueLatest, DateTime requestDate, bool isDay);
    }

    public class NotificationServiceCommonImp(
        RocketChatService rocketChatService,
        CacheManipulatorCommon cacheManipulator,
        IHttpClientFactory httpClientFactory,
        IDynamicRepository dynamicRepository,
        IConfiguration configuration,
        ILogger logger) : NotificationServiceCommon
    {
        private HttpClient client;
        private const int CACHE_EXPIRED_IN_3_DAYS = 3;
        private const int batchSize = 20;
        private const string ChannelName = "#New_League_Need_Assigned";
        
        private static readonly HashSet<string> AllowSabaLeagues = new(StringComparer.OrdinalIgnoreCase)
        {
            "SABA CLUB FRIENDLY Virtual FC 24 - 16 Mins Play",
            "SABA INTERNATIONAL FRIENDLY Virtual PES 21 - 20 Mins Play",
            "SABA CLUB FRIENDLY Virtual PES 21 - 15 Mins Play",
            "SABA INTERNATIONAL FRIENDLY Virtual FC 24 - 16 Mins Play",
            "SABA SPAIN PRIMERA LALIGA Virtual PES 24 - 15 Mins Play",
            "SABA ENGLISH PREMIER LEAGUE Virtual PES 24 - 15 Mins Play",
            "SABA VIETNAM V LEAGUE 1 Virtual PES 24 - 15 Mins Play"
        };

        public async Task SendNotificationForNewLeague(IList<string> leagueLatest, DateTime requestDate, bool isDay)
        {
            try
            {

                var cacheKey = Utils.BuildPrematchNotificationCacheKey(requestDate, isDay);
                var currentCacheInfo = await cacheManipulator.GetSingleData(cacheKey, new CacheInfo());

                var newLeague = !currentCacheInfo.Leagues.Any()
                    ? []
                    : leagueLatest.Except(currentCacheInfo.Leagues, StringComparer.OrdinalIgnoreCase).ToList();

                if (!newLeague.Any())
                {
                    return;
                }

                var filteredLeagues = FilterAllowedLeagues(newLeague);

                // Send notification for new leagues
                if (filteredLeagues.Any())
                {
                    client = rocketChatService.CreateClient();
                    await rocketChatService.LoginToRc(client);
                    await MessageToChannelInBatches(filteredLeagues, isDay, requestDate);
                    await rocketChatService.LogOutRC(client);
                }
                var newCache = JsonConvert.SerializeObject(new CacheInfo
                {
                    Actor = currentCacheInfo.Actor,
                    Leagues = newLeague.Union(currentCacheInfo.Leagues).ToList(),
                });
               
                await cacheManipulator.Cache(cacheKey, newCache,
                    TimeSpan.FromDays(CACHE_EXPIRED_IN_3_DAYS));

                await LogDetailedPayload(DateTimeConverter.ToGmt8(DateTime.Now), "[Prematch_Notification]", newCache);
            }
            catch (RocketChatException ex)
            {
                await LogDetailedPayload(DateTimeConverter.ToGmt8(DateTime.Now), "[PreMatchNotification] - error",
                    ex.ToString());

                await logger.ErrorAsync(ex.Message, ex);
            }
            catch (Exception e)
            {
                await logger.ErrorAsync(e.Message, e);
            }
        }

        public async Task MessageToChannelInBatches(IList<string> leagues, bool isDay, DateTime RequestDate)
        {
            

            for (var i = 0; i < leagues.Count; i += batchSize)
            {
                try
                {
                    var batch = leagues.Skip(i).Take(batchSize).ToList();
                    var shiftType = isDay ? "DAYSHIFT" : "NIGHTSHIFT";
                    var messages = batch.Select(league => 
                        $"[{shiftType}] **New League Alert**\n The league **{league}** is currently unassigned.");
                    
                    var message = new
                    {
                        channel = ChannelName,
                        text = string.Join("\n-------------------\n", messages)
                    };
                    await SendMessageToChannel(message, RequestDate);
                }
                catch (Exception ex)
                {
                    await LogDetailedPayload(DateTimeConverter.ToGmt8(DateTime.Now),
                        $"[NewLeagueNotification]-MessageToChannelInBatches: {ChannelName} - Batch failed",
                        System.Text.Json.JsonSerializer.Serialize(ex));
                }
            }
        }


        private async Task SendMessageToChannel(object message, DateTime RequestDate)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/v1/chat.postMessage", content).ConfigureAwait(false);
                var logAction = $"[NewLeagueNotification]-MessageToChannel: {ChannelName} - Success";

                if (!response.IsSuccessStatusCode)
                {
                    logAction = $"[NewLeagueNotification]-MessageToChannel: {ChannelName} - Fail";
                }
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var logInfo = new
                {
                    leagues = json,
                    RequestDate,
                    channel = ChannelName,
                    message = response,
                    responseBody
                };
                await LogDetailedPayload(DateTimeConverter.ToGmt8(DateTime.Now), logAction, System.Text.Json.JsonSerializer.Serialize(logInfo)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await LogDetailedPayload(DateTimeConverter.ToGmt8(DateTime.Now), $"[NewLeagueNotification]-MessageToChannel {ChannelName} - Error", ex.Message).ConfigureAwait(false);
                await logger.ErrorAsync(ex.Message, ex);
            }
        }


        private async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
        {
            var owpLogCriteria = new OwpLogCriteria
            {
                CreatedDate = CreatedDate,
                Action = Action,
                Payload = Payload
            };
            await dynamicRepository.ExecuteAsync(owpLogCriteria);
        }
       
        private static IList<string> FilterAllowedLeagues(IList<string> leagues)
        {
            return leagues.Where(league => 
            {
                if (league.Contains("saba", StringComparison.OrdinalIgnoreCase))
                {
                    return AllowSabaLeagues.Contains(league);
                }
                return true;
            }).ToList();
        }
    }
}
