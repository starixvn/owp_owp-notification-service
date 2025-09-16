using System.Text;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.PreMatchNotification.Shared.Services;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using OWP.Core.Exceptions;
using System.Net.Http.Json;
using Olympus.OWP.Cache;
using OWP.PreMatchNotification.NewLeague.Models;

namespace OWP.PreMatchNotification.NewLeague.Services
{
    public interface NotificationServiceCommon
    {
        Task SendNotificationForNewLeague(IList<string> leagueLatest, DateTime requestDate, bool isDay);
    }

    public class NotificationServiceCommonImp(
        CacheManipulatorCommon cacheManipulator,
        IHttpClientFactory httpClientFactory,
        IDynamicRepository dynamicRepository,
        IConfiguration configuration,
        ILogger logger) : NotificationServiceCommon
    {
        private HttpClient client;
        private const string PREMATCH_SHIFT = "OWP-prematch-shift";
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
            var cacheKey = Utils.BuildPrematchNotificationCacheKey(requestDate, isDay);
            var leagueInCache = await cacheManipulator.GetMultipleData<string>(cacheKey);

            var newLeague = !leagueInCache.Any()
                ? []
                : leagueLatest.Except(leagueInCache, StringComparer.OrdinalIgnoreCase).ToList();

            var filteredLeagues = FilterAllowedLeagues(newLeague);
            
            // Send notification for new leagues
            if (filteredLeagues.Any())
            {
                CreateClient();
                await LoginToRc();
                await MessageToChannelInBatches(filteredLeagues, isDay, requestDate);
                await LogOutRC();
            }

            var assignmentSnapshotInJson = JsonConvert.SerializeObject(newLeague.Union(leagueInCache));
            await cacheManipulator.Cache(cacheKey, assignmentSnapshotInJson, TimeSpan.FromDays(CACHE_EXPIRED_IN_3_DAYS));
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
        
        private static DateTime ToGmt8(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().AddHours(8);
        }
        private static IList<string> FilterAllowedLeagues(IList<string> leagues)
        {
            return leagues.Where(league => !ExcludeSabaLeagues.Contains(league)).ToList();
        }

        private void CreateClient()
        {
            var chatUrl = configuration["ChatServer:Url"];
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(chatUrl);
            client.Timeout = TimeSpan.FromMinutes(2);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        private async Task LogOutRC()
        {
            await client.PostAsync("api/v1/logout", null);
        }

        private async Task LoginToRc()
        {
            try
            {
                var username = configuration["ChatServer:BotName"];
                var password = configuration["ChatServer:BotPassword"];
                var loginPayload = new { user = username, password };

                var response = await client.PostAsJsonAsync("api/v1/login", loginPayload);
                response.EnsureSuccessStatusCode();

                var authResult = await response.Content.ReadFromJsonAsync<RocketChatAuthResponse>();

                client.DefaultRequestHeaders.Add("X-Auth-Token", authResult.Data.AuthToken);
                client.DefaultRequestHeaders.Add("X-User-Id", authResult.Data.UserId);
            }
            catch (Exception exception)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]- Login - Fatal error", exception.ToString());

                throw new RocketChatException("Failed to login to Rocket.Chat", exception);
            }
        }
    }
}
