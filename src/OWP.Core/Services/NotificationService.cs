using OWP.Core.Models;
using OWP.Hubs;

namespace OWP.Core.Services;

using Microsoft.AspNetCore.SignalR;

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
                message = message.Message
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

