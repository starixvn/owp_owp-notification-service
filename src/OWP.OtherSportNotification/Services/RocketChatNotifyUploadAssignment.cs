using Fanex.Caching;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.OtherSport;

namespace OWP.OtherSportNotification.Services
{
    public interface RocketChatNotifyUploadAssignment
    {
        Task MessageToChannel(HttpClient client, OtherSportNotificationRequest request);
    }

    public class RocketChatNotifyUploadAssignmentImp(
        RocketChatMessageToChannel messageToChannel,
        ICache cache) : RocketChatNotifyUploadAssignment
    {
        private const string ChannelName = "#LC_Update_Notification";
        private const string KEY_OS_UPLOAD_LAST_UPDATED = "OWP-OS-AssignmentUpload-Last-Updated";

        public async Task MessageToChannel(HttpClient client, OtherSportNotificationRequest request)
        {
            var message = new
            {
                channel = ChannelName,
                text = $"Hi all,\nLC updated (Last updated on **{await GetLastUpdate()} (GMT+8)** by **{request.UserName}**).\nPlease download the LC latest version.\nThanks"
            };
            await messageToChannel.MessageToChannel(client, message, request);
        }

        public async Task<string> GetLastUpdate()
        {
            var (_, value) = await cache.TryGetAsync<string>(KEY_OS_UPLOAD_LAST_UPDATED);

            var lastUpdated = string.IsNullOrEmpty(value)
                ? new LastUpdateInfo()
                : JsonConvert.DeserializeObject<LastUpdateInfo>(value);

            return lastUpdated.Date.ToString("MMMM dd, yyyy hh:mm tt");
        }
    }
}
