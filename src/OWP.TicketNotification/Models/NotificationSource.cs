namespace OWP.TicketNotification.Models;

public class NotificationSource
{
    public string SessionId { get; set; }

    public int TicketId { get; set; }

    public int EventType { get; set; }

    public bool IsNewMessage { get; set; }
}
