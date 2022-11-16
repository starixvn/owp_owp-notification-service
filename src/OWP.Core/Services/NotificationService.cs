using Microsoft.AspNetCore.SignalR;
using OWP.Hubs;
using  OWP.Core.Models;

namespace OWP.Core.Services;

public interface NotificationService
{
    Task Notify<T>(IList<MessageModelCommon<T>> sources) where T : class;
}

public class NotificationServiceImp : NotificationService
{
    private readonly IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext;

    public NotificationServiceImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    public async Task Notify<T>(IList<MessageModelCommon<T>> sources) where T : class
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

