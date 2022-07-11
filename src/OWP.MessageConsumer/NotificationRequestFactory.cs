using Newtonsoft.Json;
using OWP.Core.NotificationRequests.Prematch;
using OWP.MessageConsumer.Constants;

namespace OWP.Core.NotificationRequests;
public class NotificationRequestFactory
{
    public static NotificationRequest GetNotificationRequest(string key, string message)
        => key switch
        {
            RoutingKey.TicketCenter => JsonConvert.DeserializeObject<NewTicketNotificationRequest>(message),
            RoutingKey.LinkedLgaMatch => JsonConvert.DeserializeObject<LinkedMatchNotifyRequest>(message),
            RoutingKey.InfositeMatchInfo => JsonConvert.DeserializeObject<MatchInfoNotifyRequest>(message),
            RoutingKey.PrematchAssignment => JsonConvert.DeserializeObject<PrematchAssignmentNotifyRequest>(message),
            RoutingKey.PrematchNewLeague => JsonConvert.DeserializeObject<PrematchNewLeagueNotifyRequest>(message),
            RoutingKey.OtherSport => JsonConvert.DeserializeObject<OtherSportNotificationRequest>(message),

            _ => new NotificationRequest()
        };
}
