using System.Text;
using Fanex.Data.Repository;
using MediatR;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.PreMatchNotification.Shared.Services;


namespace OWP.PreMatchNotification.NewLeague.Services
{
    public interface NotificationServiceCommon
    {
        Task SendNotificationForNewLeague(IList<string> leagueLatest, DateTime requestDate, bool isDay);
    }

    public class NotificationServiceCommonImp(
        CacheManipulatorCommon cacheManipulator,
        IHttpClientFactory httpClientFactory,
        IDynamicRepository dynamicRepository) : NotificationServiceCommon
    {
        private const string PREMATCH_SHIFT = "prematch-shift";
        private const int CACHE_EXPIRED_IN_3_DAYS = 3;
        private const int batchSize = 20;
        private const string ChannelName = "#New_League_Need_Assigned";
        
        private static readonly HashSet<string> ExcludeSabaLeagues = new(StringComparer.OrdinalIgnoreCase)
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
            var cacheKey = BuildKey(requestDate, isDay);
            var leagueInCache = await cacheManipulator.GetMultipleData<string>(cacheKey);

            var newLeague = !leagueInCache.Any()
                ? []
                : leagueLatest.Except(leagueInCache, StringComparer.OrdinalIgnoreCase).ToList();
            
            var filteredLeagues = FilterAllowedLeagues(newLeague);
            
            // Send notification for new leagues
            if (filteredLeagues.Any())
            {
                await MessageToChannelInBatches(filteredLeagues, isDay, requestDate);
            }

            if (cacheManipulator.ContainKey(cacheKey))
            {
                await cacheManipulator.RemoveData(cacheKey);
            }
            var assignmentSnapshotInJson = JsonConvert.SerializeObject(filteredLeagues.Union(leagueInCache));
            await cacheManipulator.Cache(assignmentSnapshotInJson, cacheKey, TimeSpan.FromMinutes(CACHE_EXPIRED_IN_3_DAYS));
        }

        private static string BuildKey(DateTime requestDate, bool isDay)
            => isDay
                ? $"{PREMATCH_SHIFT}-{requestDate:yyyy-MM-dd}-day-shift"
                : $"{PREMATCH_SHIFT}-{requestDate:yyyy-MM-dd}-night-shift";


        public async Task MessageToChannelInBatches(IList<string> leagues, bool isDay, DateTime RequestDate)
        {
            using var client = httpClientFactory.CreateClient("OWPChatBot");
            
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
                    await SendMessageToChannel(client, message, RequestDate);
                }
                catch (Exception ex)
                {
                    await LogDetailedPayload(ToGmt8(DateTime.Now),
                        $"[NewLeagueNotification]-MessageToChannelInBatches: {ChannelName} - Batch failed",
                        System.Text.Json.JsonSerializer.Serialize(ex));
                }
            }
        }


        private async Task SendMessageToChannel(HttpClient client, object message, DateTime RequestDate)
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
                await LogDetailedPayload(ToGmt8(DateTime.Now), logAction, System.Text.Json.JsonSerializer.Serialize(logInfo)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), $"[NewLeagueNotification]-MessageToChannel {ChannelName} - Error", ex.Message).ConfigureAwait(false);
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
        
        private static DateTime ToGmt8(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().AddHours(8);
        }
        private static IList<string> FilterAllowedLeagues(IList<string> leagues)
        {
            return leagues.Where(league => !ExcludeSabaLeagues.Contains(league)).ToList();
        }
    }
}
