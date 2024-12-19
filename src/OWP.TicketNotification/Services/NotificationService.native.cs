using Microsoft.AspNetCore.SignalR;
using OWP.Hubs;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NotificationService
{
    Task Notify(List<NotificationModel> sources);
}


public class NotificationServiceImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext)
    : NotificationService
{
    public async Task Notify(List<NotificationModel> sources)
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
