using MediatR;

namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewTicketOtherNotificationRequest : NotificationRequest, IRequest<List<object>>
{
    public int TicketId { get; set; }
    public string Comment { get; set; }
    public int ActorId { get; set; }
}
