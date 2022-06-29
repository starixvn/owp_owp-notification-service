using Microsoft.AspNetCore.SignalR;
using OWP.NotificationHandler.Hubs;

namespace OWP.NotificationHandler.Services;

public interface NotifyEngine
{
    Task Notify<T>(List<T> messages, List<string> receivedUserIds) where T : class;
}

public class NotifyEngineImp : NotifyEngine
{
    private readonly IHubContext<NotificationHub> notificationHub;

    public NotifyEngineImp(IHubContext<NotificationHub> notificationHub)
    {
        this.notificationHub = notificationHub;
    }

    public Task Notify<T>(IList<T> messages, IList<string> receivedUserIds) where T : class
    {
        throw new NotImplementedException();
    }
}