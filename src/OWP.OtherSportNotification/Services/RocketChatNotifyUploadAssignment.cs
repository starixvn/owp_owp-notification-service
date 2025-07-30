using Fanex.Caching;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using System.Net.Http.Json;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;

namespace OWP.OtherSportNotification.Services
{
    public interface RocketChatNotifyUploadAssignment
    {
        Task MessageToChannel(HttpClient client, string userName, string sessionId);
    }

    public class RocketChatNotifyUploadAssignmentImp(
        ICache cache,
        OtherSportRepository repository) : RocketChatNotifyUploadAssignment
    {
        private const string ChannelName = "#LC_Update_Notification";
        private const int maxRetries = 3;
        private const int retryDelaySeconds = 2;
        private const string KEY_OS_UPLOAD_LAST_UPDATED = "OWP-OS-AssignmentUpload-Last-Updated";

        public async Task MessageToChannel(HttpClient client, string userName, string sessionId)
        {
            try
            {
                var messageResponse = await SendChannelMessageWithRetry(client, userName);
                var logInfo = new
                {
                    sessionId = sessionId,
                    uploadBy = userName,
                    channel = ChannelName,
                    message = messageResponse
                };

                var logAction = "[OsNotification]-MessageToChannel: LC_Update_Notification - Success";
                if (!messageResponse.Success)
                {
                    logAction = "[OsNotification]-MessageToChannel: LC_Update_Notification - Fail";
                }
                await LogDetailedPayload(ToGmt8(DateTime.Now), logAction,
                    System.Text.Json.JsonSerializer.Serialize(logInfo));
            }
            catch (Exception ex)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex.ToString());
                throw new Exception("[OsNotification]-MessageToChannel: LC_Update_Notification-Fatal error", ex);
            }
        }

        public async Task<string> GetLastUpdate()
        {
            var (_, value) = await cache.TryGetAsync<string>(KEY_OS_UPLOAD_LAST_UPDATED);

            var lastUpdated = string.IsNullOrEmpty(value)
                ? new LastUpdateInfo()
                : JsonConvert.DeserializeObject<LastUpdateInfo>(value);

            return lastUpdated.Date.ToString("MMMM dd, yyyy hh:mm tt");
        }

        private async Task<RCResponse> SendChannelMessageWithRetry(
            HttpClient client,
            string userName)
        {
            var rcResponse = new RCResponse();
            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                var message = new
                {
                    channel = ChannelName,
                    text = $"Hi all,\nLC updated (Last updated on **{await GetLastUpdate()} (GMT+8)** by **{userName}**).\nPlease download the LC latest version.\nThanks"
                };

                try
                {
                    var response = await client.PostAsJsonAsync("api/v1/chat.postMessage", message);

                    if (response.IsSuccessStatusCode)
                    {
                        rcResponse.Success = true;
                        rcResponse.Message.Add("Send message to trader successfully");
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
            await repository.WriteLog(new OwpLogCriteria()
            {
                CreatedDate = CreatedDate,
                Action = Action,
                Payload = Payload
            });
        }

    }
}
