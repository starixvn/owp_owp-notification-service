namespace OWP.TicketNotification.Criteria;
public class InsertNewTicketMatchNotificationCriteria : InsertNotificationMessageCriteria
{
    public int TicketId { get; set; }

    public override string GetSettingKey() => "OWP_TicketCenter_InsertNewTicketMatchNotification";

    public override bool IsValid()
        => NotificationContent.Length > 0 &&
           TicketId > 0;
}