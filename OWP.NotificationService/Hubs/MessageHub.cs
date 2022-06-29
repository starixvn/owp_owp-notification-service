using Microsoft.AspNetCore.SignalR;

namespace OWP.NotificationService.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMQMessage(List<string> messages)
        {
            await Clients.All.SendAsync("ReceiveMQMessage", messages);
        }
    }
}
