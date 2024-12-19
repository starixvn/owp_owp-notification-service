using Microsoft.AspNetCore.SignalR;
using OWP.Core.Models;
using OWP.Hubs;

namespace OWP.LiveDetailNotification.Services;

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
                    .Users($"{message.UserId}")
#endif
                .notifyChangedInformationData(result);
        }
    }
}