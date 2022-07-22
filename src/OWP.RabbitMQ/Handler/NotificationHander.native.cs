using MediatR;
using Newtonsoft.Json;
using OWP.Core.Contants;
using OWP.Core.NotificationRequests;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.Core.NotificationRequests.Prematch;
using OWP.Core.NotificationRequests.TicketCenter;

namespace OWP.RabbitMQ.Handler;

public interface NotificationHander
{
    Task<string> Handle(string key, string message);
}

public class SendNotificationHanderImp : NotificationHander
{
    private readonly IMediator mediator;

    public SendNotificationHanderImp(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<string> Handle(string key, string message)
    {
        var request = MapRequest(key, message);
        return await mediator.Send(request);
    }

    public static NotificationRequest MapRequest(string key, string message) => key switch
    {
        RoutingKey.TicketCenter => JsonConvert.DeserializeObject<NewTicketEventNotificationRequest>(message),

        RoutingKey.LinkedLgaMatch => JsonConvert.DeserializeObject<LinkedMatchNotifyRequest>(message),

        RoutingKey.InfositeMatchInfo => JsonConvert.DeserializeObject<MatchInfoNotifyRequest>(message),

        RoutingKey.PrematchAssignment => JsonConvert.DeserializeObject<PrematchAssignmentNotifyRequest>(message),

        RoutingKey.PrematchNewLeague => JsonConvert.DeserializeObject<PrematchNewLeagueNotifyRequest>(message),

        RoutingKey.OtherSport => JsonConvert.DeserializeObject<OtherSportNotificationRequest>(message),

        RoutingKey.TicketCenterNewMessage => JsonConvert.DeserializeObject<NewMessageNotificationRequest>(message),

        RoutingKey.TicketCenterNewRecipient => JsonConvert.DeserializeObject<NewRecipientNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketEvent => JsonConvert.DeserializeObject<NewTicketEventNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketLeague => JsonConvert.DeserializeObject<NewTicketLeagueNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketOther => JsonConvert.DeserializeObject<NewTicketOtherNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketSupport => JsonConvert.DeserializeObject<NewTicketSupportNoticationRequest>(message),

        _ => new NotificationRequest()
    };
}
