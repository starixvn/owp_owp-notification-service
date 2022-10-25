namespace OWP.Core.NotificationRequests.Prematch;

public class MatchInfoNotifyRequest : NotificationRequest
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
}