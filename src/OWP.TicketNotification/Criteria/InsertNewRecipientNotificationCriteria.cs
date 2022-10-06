namespace OWP.TicketNotification.Criteria;
public class InsertNewRecipientNotificationCriteria : InsertNotificationMessageCriteria
{
    public int ReceivedUserId { get; set; }

    public int ReceivedUserType { get; set; }

    public override string GetSettingKey() => "OWP_TicketCenter_InsertNewRecipientNotification";

    public override bool IsValid()
        => NotificationContent.Length > 0 && ReceivedUserId > 0;
}
