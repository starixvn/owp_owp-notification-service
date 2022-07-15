using MediatR;

namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewTicketEventNotificationRequest : NotificationRequest, IRequest<Unit>
{
    public int ActorId { get; set; }
    public int TicketId { get; set; }
    public string Comment { get; set; }
}