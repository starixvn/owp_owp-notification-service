using MediatR;
using Newtonsoft.Json;
using OWP.Core.Constants;
using OWP.Core.NotificationRequests;
using OWP.Core.NotificationRequests.AssignmentUpload;
using OWP.Core.NotificationRequests.LiveDetail;
using OWP.Core.NotificationRequests.OddsCompiler;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.Core.NotificationRequests.Prematch;
using OWP.Core.NotificationRequests.TicketCenter;

namespace OWP.RabbitMQ.Handler;

public interface NotificationHandler
{
    Task Handle(string key, string message);
}

public class SendNotificationHandlerImp(IMediator mediator) : NotificationHandler
{
    public async Task Handle(string key, string message)
    {
        var request = MapRequest(key, message);

        await mediator.Send(request);
    }

    public static NotificationRequest MapRequest(string key, string message) => key switch
    {
        RoutingKey.AssignmentUpload => JsonConvert.DeserializeObject<AssignmentUploadNotifyRequest>(message),

        RoutingKey.LinkedLgaMatch => JsonConvert.DeserializeObject<LinkedMatchNotifyRequest>(message),

        RoutingKey.PreMatchAssignment => JsonConvert.DeserializeObject<PrematchAssignmentNotifyRequest>(message),

        RoutingKey.PreMatchNewLeague => JsonConvert.DeserializeObject<PrematchNewLeagueNotifyRequest>(message),

        RoutingKey.OtherSport => JsonConvert.DeserializeObject<OtherSportNotificationRequest>(message),

        RoutingKey.TicketCenterNewMessage => JsonConvert.DeserializeObject<NewMessageNotificationRequest>(message),

        RoutingKey.TicketCenterNewRecipient => JsonConvert.DeserializeObject<NewRecipientNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketLiveIncident => JsonConvert.DeserializeObject<NewTicketLiveIncidentNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketEvent => JsonConvert.DeserializeObject<NewTicketEventNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketLeague => JsonConvert.DeserializeObject<NewTicketLeagueNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketOther => JsonConvert.DeserializeObject<NewTicketOtherNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketSupport => JsonConvert.DeserializeObject<NewTicketSupportNotificationRequest>(message),

        RoutingKey.TicketCenterNewTicketLogWork => JsonConvert.DeserializeObject<NewTicketLogWorkNotificationRequest>(message),

        RoutingKey.LiveDetail => JsonConvert.DeserializeObject<LiveDetailNotifyRequest>(message),
        
        RoutingKey.OddsCompiler => JsonConvert.DeserializeObject<OddsCompilerNotifyRequest>(message),

        _ => new NotificationRequest()
    };
}
