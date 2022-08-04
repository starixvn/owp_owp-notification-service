using Fanex.Logging;
using Microsoft.AspNetCore.SignalR;
using OWP.Hubs;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Shared.Services;

public interface NotificationService
{
    Task Notify(List<MessageModel> sources);
}

public class NotificationServiceImp : NotificationService
{
    private readonly IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext;
    private readonly ILogger logger;

    public NotificationServiceImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext, ILogger logger)
    {
        this.hubContext = hubContext;
        this.logger = logger;
    }

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
                .Users(message.ReceivedUserIdList)
#endif
                .notifyChangedInformationData(result);
        }
    }
}
