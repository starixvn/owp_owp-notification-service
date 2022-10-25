namespace OWP.Core.NotificationRequests.TicketCenter;

public class NewTicketLiveIncidentNotificationRequest : NotificationRequest
{
    public int ActorId { get; set; }
    public int TicketId { get; set; }
    public string Comment { get; set; }
    public string OwpLeagueName { get; set; }
    public string OwpHomeName { get; set; }
    public string OwpAwayName { get; set; }
}