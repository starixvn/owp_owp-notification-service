using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.OtherSportNotification.Models;
using Fanex.Logging;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.Core.Exceptions;
using OWP.Core.Services;
using OWP.Core.Utils;

namespace OWP.OtherSportNotification.Services
{
    public interface MessageToRocketChat
    {
        Task Send(IList<ChangedValue> changedValues, OtherSportNotificationRequest request);
    }

    public class MessageToRocketChatImp(
        RocketChatService rocketChatService,
    ILogger logger,
    RocketChatNotifyAssignmentChange notifyAssignmentChange,
    RocketChatNotifyUploadAssignment notifyUploadAssignment,
    RocketChatNotifyEventUnlink notifyEventUnlink,
    OtherSportRepository repository) : MessageToRocketChat
    {
        public async Task Send(IList<ChangedValue> changedValues, OtherSportNotificationRequest request)
        {
            try
            {
                var client = rocketChatService.CreateClient();
                await rocketChatService.LoginToRc(client);
                await notifyUploadAssignment.MessageToChannel(client, request);
                await notifyAssignmentChange.MessageToTrader(client, changedValues, request);
                await notifyEventUnlink.SendMessageUnlinkEvent(client, request);
                await rocketChatService.LogOutRC(client);
            }
            catch (RocketChatException ex)
            {
                await LogDetailedPayload(DateTimeConverter.ToGmt8(DateTime.Now), "[OsNotification] - error", ex.ToString());

                await logger.ErrorAsync(ex.Message, ex);
            }
            catch (Exception e)
            {
                await logger.ErrorAsync(e.Message, e);

            }
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
    }
}
