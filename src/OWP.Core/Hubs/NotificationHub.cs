using Microsoft.AspNetCore.SignalR;

namespace OWP.Core.Hubs;

public class NotificationHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("notifyChangedInformationData", message);
    }
}
