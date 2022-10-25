namespace OWP.TicketNotification.Criteria;

public class InsertNewMessageNotificationCriteria : InsertNotificationMessageCriteria
{
    public int TicketId { get; set; }

    public override string GetSettingKey() => "OWP_TicketCenter_InsertNewMessageNotification";

    public override bool IsValid()
        => NotificationContent.Length > 0 &&
           TicketId > 0;
}
