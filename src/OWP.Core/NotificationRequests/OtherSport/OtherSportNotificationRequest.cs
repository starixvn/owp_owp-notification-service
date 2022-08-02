namespace OWP.Core.NotificationRequests.OtherSport;

public class OtherSportNotificationRequest : NotificationRequest
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
    public int ActorId { get; set; }
    public string UserName { get; set; }
}
