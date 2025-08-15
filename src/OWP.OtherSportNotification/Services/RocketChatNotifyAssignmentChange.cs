using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.OtherSportNotification.Models;
using System.Net.Http.Json;
using System.Text.Json;
using OWP.Core.Exceptions;
using OWP.Core.NotificationRequests.OtherSport;

namespace OWP.OtherSportNotification.Services
{
    public interface RocketChatNotifyAssignmentChange
    {
        Task MessageToTrader(
            HttpClient client,
            IList<ChangedValue> changedValues,
            OtherSportNotificationRequest request);
    }

    public class RocketChatNotifyAssignmentChangeImp(
        RocketChatMessageFactory rocketChatMessageFactory,
        OtherSportRepository repository,
        RocketChatMessageToChannel messageToChannel) : RocketChatNotifyAssignmentChange
    {
        public async Task MessageToTrader(
            HttpClient client,
            IList<ChangedValue> changedValues,
            OtherSportNotificationRequest request)
        {
            try
            {
                if (!changedValues.Any())
                {
                    await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Success", "Nothing changed");
                    return;
                }

                var messageModels = await rocketChatMessageFactory.BuildMessage(changedValues);
                var rcResponseSuccess = new List<object>();
                var rcResponseFail = new List<object>();

                foreach (var message in messageModels)
                {
                    var messageResponse = await SendMessageWithRetry(client, message);
                    if (messageResponse.Success)
                    {
                        rcResponseSuccess.Add(new
                        {
                            recipient = message.Recipient,
                            message = messageResponse,
                        });
                    }
                    else
                    {
                        rcResponseFail.Add(new
                        {
                            recipient = message.Recipient,
                            message = messageResponse,
                        });
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                await LogRcResponse("[OsNotification]-MessageToTrader - Success", rcResponseSuccess, request.UserName, request.SessionId);
                await LogRcResponse("[OsNotification]-MessageToTrader - Fail", rcResponseFail, request.UserName, request.SessionId);
            }
            catch (Exception ex)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[OsNotification]-MessageToTrader - Fatal error", ex.ToString());
                throw new RocketChatException("[OsNotification]-MessageToTrader - Fatal error", ex);
            }
        }

        private async Task LogRcResponse(
            string action, IList<object> payload, string uploadBy, string sessionId)
        {
            if (payload.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), action,
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        sessionId,
                        uploadBy,
                        payload
                    }));
            }

        }
        private async Task<RCResponse> SendMessageWithRetry(
            HttpClient client,
            RocketChatMessageModel message)
        {
            var imCreateResponse = await client.PostAsJsonAsync("api/v1/im.create", new { username = message.Recipient });
            var imResult = await imCreateResponse.Content.ReadFromJsonAsync<JsonElement>();
            var roomId = imResult.GetProperty("room").GetProperty("_id").GetString();

            var messagePayload = new { roomId, text = message.Message };
            return await messageToChannel.SendChannelMessageWithRetry(client, messagePayload);
        }

        private async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
        {
            await repository.WriteLog(new OwpLogCriteria
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
