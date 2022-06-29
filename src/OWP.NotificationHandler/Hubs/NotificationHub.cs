using Microsoft.AspNetCore.SignalR;

namespace OWP.NotificationHandler.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotifycation(string message, List<string> usersId)
    {
        await Clients.Users(usersId).SendAsync(message);
    }
}
