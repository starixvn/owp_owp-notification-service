namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewTicketLogWorkNotificationRequest : NotificationRequest
{
    public int ActorId { get; set; }
    public int TicketId { get; set; }
    public string Comment { get; set; }
}