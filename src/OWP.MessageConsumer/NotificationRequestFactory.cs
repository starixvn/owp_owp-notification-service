using Newtonsoft.Json;
using OWP.MessageConsumer.Constants;

namespace OWP.Core.NotificationRequests;
public class NotificationRequestFactory
{
    public static NotificationRequest GetNotificationRequest(string key, string message)
        => key switch
        {
            RoutingKey.TicketCenter => JsonConvert.DeserializeObject<NewTicketNotificationRequest>(message),
            RoutingKey.OtherSport => JsonConvert.DeserializeObject<OtherSportNotificationRequest>(message),
            _ => new NotificationRequest()
        };
}
