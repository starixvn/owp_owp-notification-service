using MediatR;

namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewMessageNotificationRequest : NotificationRequest, IRequest<List<object>>
{
    public string Message { get; set; }

    public int? FileId { get; set; }

    public int TicketEventType { get; set; }

    public string TicketTitle { get; set; }
    public int ActorId { get; set; }
    public int TicketId { get; set; }
}
