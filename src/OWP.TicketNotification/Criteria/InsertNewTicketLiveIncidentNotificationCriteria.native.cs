namespace OWP.TicketNotification.Criteria;

public class InsertNewTicketLiveIncidentNotificationCriteria : InsertNotificationMessageCriteria
{
    public int TicketId { get; set; }

    public string OwpLeagueName { get; set; }

    public string OwpHomeName { get; set; }

    public string OwpAwayName { get; set; }

    public override string GetSettingKey()
    {
        return "OWP_TicketCenter_InsertNewTicketLiveIncidentNotification";
    }

    public override bool IsValid()
    {
        return NotificationContent.Length > 0 &&
               TicketId > 0;
    }
}