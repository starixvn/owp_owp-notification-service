using OWP.Core.Models;

namespace OWP.TicketNotification.Models;

public class NotificationModel : MessageModel
{
    public int TicketId { get; set; }

    public int TicketType { get; set; }

    public int EventType { get; set; }

    public int? TotalMessageUnread { get; set; }

    public bool IsNewMessage { get; set; }
}
