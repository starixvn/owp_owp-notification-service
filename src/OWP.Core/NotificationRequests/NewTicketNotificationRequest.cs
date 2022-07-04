using MediatR;

namespace OWP.Core.NotificationRequests;

public class NewTicketNotificationRequest : NotificationRequest, IRequest<Unit>
{
    public int TicketId { get; set; }

    public int EventType { get; set; }
}
