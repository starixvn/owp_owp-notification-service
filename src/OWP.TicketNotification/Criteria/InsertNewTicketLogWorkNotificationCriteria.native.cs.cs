namespace OWP.TicketNotification.Criteria;

public class InsertNewTicketLogWorkNotificationCriteria : InsertNotificationMessageCriteria
{
    public int TicketId { get; set; }

    public override string GetSettingKey() => "OWP_TicketCenter_InsertNewTicketLogWorkNotification";

    public override bool IsValid()
        => NotificationContent.Length > 0 &&
           TicketId > 0;
}