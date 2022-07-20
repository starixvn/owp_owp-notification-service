using MediatR;

namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewTicketLeagueNotificationRequest : NotificationRequest, IRequest<List<object>>
{
    public int CreatorId { get; set; }
    public int TicketId { get; set; }
    public string Comment { get; set; }
}
