using Microsoft.AspNetCore.SignalR;
using OWP.Hubs;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NotificationService
{
    Task Notify(List<NotificationModel> sources);
}


public class NotificationServiceImp : NotificationService
{
    private readonly IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext;

    public NotificationServiceImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext)
    {
        this.hubContext = hubContext;
    }

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
