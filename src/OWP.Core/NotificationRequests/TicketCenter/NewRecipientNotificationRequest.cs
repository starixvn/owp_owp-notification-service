using MediatR;

namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewRecipientNotificationRequest : NotificationRequest, IRequest<List<object>>
{
    public string TicketTitle { get; set; }

    public int TicketEventType { get; set; }

    public int RecipientId { get; set; }

    public int RecipientType { get; set; }
    public int ActorId { get; set; }
    public int TicketId { get; set; }
}
