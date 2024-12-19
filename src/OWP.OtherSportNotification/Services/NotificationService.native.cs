using Microsoft.AspNetCore.SignalR;
using OWP.Hubs;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface NotificationService
{
    Task Notify(List<MessageModel> sources);
}

public class NotificationServiceImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext)
    : NotificationService
{
    public async Task Notify(List<MessageModel> sources)
    {
        foreach (var message in sources)
        {
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message
            };
            await hubContext.Clients
#if DEBUG
                .All
#else
                    .Users(message.ReceivedUserIds)
#endif
                .notifyChangedInformationData(result);
        }
    }
}