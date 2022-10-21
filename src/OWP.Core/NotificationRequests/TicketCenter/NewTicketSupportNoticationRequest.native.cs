namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewTicketSupportNoticationRequest : NotificationRequest
{
    public int TicketId { get; set; }
    public string Comment { get; set; }
    public int ActorId { get; set; }
}
