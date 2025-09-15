using Olympus.OWP.Log.Data.Repository.Criteria;
using System.Net.Http.Json;
using OWP.Core.Exceptions;
using OWP.Core.NotificationRequests.OtherSport;
using Fanex.Data.Repository;
using OWP.PreMatchNotification.NewLeague.Models;

namespace OWP.SyncPrematchLeague.Services
{
    public interface RocketChatMessageNewLeague
    {
        Task<RCResponse> SendChannelMessageWithRetry(
            HttpClient client,
            object message);
        Task MessageToChannel(HttpClient client, object message, OtherSportNotificationRequest request);
    }

    public class RocketChatMessageNewLeaguelImp(
        IDynamicRepository dynamicRepository) : RocketChatMessageNewLeague
    {
        private const string ChannelName = "#New_League_Need_Assigned";
        private const int maxRetries = 3;
        private const int retryDelaySeconds = 2;
        private const int delaySeconds = 1;
       
        public async Task MessageToChannel(HttpClient client, object message, OtherSportNotificationRequest request )
        {
            try
            {
                var messageResponse = await SendChannelMessageWithRetry(client, message);
                var logInfo = new
                {
                    sessionId = request.SessionId,
                    uploadBy = request.UserName,
                    channel = ChannelName,
                    message = messageResponse
                };

                var logAction = $"[NewLeagueNotification]-MessageToChannel: {ChannelName} - Success";
                if (!messageResponse.Success)
                {
                    logAction = $"[NewLeagueNotification]-MessageToChannel: {ChannelName} - Fail";
                }

                await LogDetailedPayload(ToGmt8(DateTime.Now), logAction,
                    System.Text.Json.JsonSerializer.Serialize(logInfo));
            }
            catch (Exception ex)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now),
                    $"[NewLeagueNotification]-MessageToChannel: {ChannelName} - Fatal error", ex.ToString());
                throw new RocketChatException("[NewLeagueNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex);
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        public async Task<RCResponse> SendChannelMessageWithRetry(
            HttpClient client,
            object message)
        {
            var rcResponse = new RCResponse();
            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var response = await client.PostAsJsonAsync("api/v1/chat.postMessage", message);

                    if (response.IsSuccessStatusCode)
                    {
                        rcResponse.Success = true;
                        rcResponse.Message.Add("Send message successfully");
                        return rcResponse;
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();

                    rcResponse.Success = false;
                    rcResponse.Message.Add($"Attempt {attempt} failed to send message reason: {responseBody}");
                }
                catch (Exception ex)
                {
                    var exMessage = $"Exception: {ex.ToString()}, message info: {System.Text.Json.JsonSerializer.Serialize(message)}";
                    rcResponse.Success = false;
                    rcResponse.Message.Add($"Attempt {attempt} exception during message send reason: {exMessage}");
                }

                if (attempt < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                }
            }

            return rcResponse;
        }

        private static DateTime ToGmt8(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().AddHours(8);
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

    }
}
