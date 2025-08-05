using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.OtherSportNotification.Models;
using System.Net.Http.Json;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using OWP.Core.NotificationRequests.OtherSport;

namespace OWP.OtherSportNotification.Services
{
    public interface MessageToRocketChat
    {
        Task Send(IList<ChangedValue> changedValues, OtherSportNotificationRequest request);
    }

    public class MessageToRocketChatImp(
    IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
    ILogger logger,
    RocketChatNotifyAssignmentChange notifyAssignmentChange,
    RocketChatNotifyUploadAssignment notifyUploadAssignment,
    RocketChatNotifyEventUnlink notifyEventUnlink,
    OtherSportRepository repository) : MessageToRocketChat
    {
        private HttpClient client;
        public async Task Send(IList<ChangedValue> changedValues, OtherSportNotificationRequest request)
        {
            try
            {
                CreateClient();
                await LoginToRc();
                await notifyUploadAssignment.MessageToChannel(client, request);
                await notifyAssignmentChange.MessageToTrader(client, changedValues, request);
                //await notifyEventUnlink.SendMessageUnlinkEvent(client, request);
                await LogOutRC();
            }
            catch (Exception e)
            {
                await logger.ErrorAsync(e.Message, e);
            }
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
                var loginPayload = new { user = username, password = password };

                var response = await client.PostAsJsonAsync("api/v1/login", loginPayload);
                response.EnsureSuccessStatusCode();

                var authResult = await response.Content.ReadFromJsonAsync<RocketChatAuthResponse>();

                client.DefaultRequestHeaders.Add("X-Auth-Token", authResult.Data.AuthToken);
                client.DefaultRequestHeaders.Add("X-User-Id", authResult.Data.UserId);
            }
            catch (Exception exception)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]- Login - Fatal error", exception.ToString());

                throw new Exception("Failed to login to Rocket.Chat", exception);
            }
        }
        
        private async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
        {
            await repository.WriteLog(new OwpLogCriteria()
            {
                CreatedDate = CreatedDate,
                Action = Action,
                Payload = Payload
            });
        }

        private static DateTime ToGmt8(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().AddHours(8);
        }
    }
}
