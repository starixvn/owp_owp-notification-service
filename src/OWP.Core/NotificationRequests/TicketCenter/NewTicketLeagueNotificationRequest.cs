namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewTicketLeagueNotificationRequest : NotificationRequest
{
    public int CreatorId { get; set; }
    public int TicketId { get; set; }
    public string Comment { get; set; }
}
