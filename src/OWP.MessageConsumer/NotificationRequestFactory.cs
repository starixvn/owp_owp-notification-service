using Newtonsoft.Json;
using OWP.MessageConsumer.Constants;

namespace OWP.Core.NotificationRequests;
public class NotificationRequestFactory
{
    public static NotificationRequest GetNotificationRequest(string key, string message)
        => key switch
        {
            RoutingKey.TicketCenter => JsonConvert.DeserializeObject<NewTicketNotificationRequest>(message),
            _ => new NotificationRequest()
        };
}
