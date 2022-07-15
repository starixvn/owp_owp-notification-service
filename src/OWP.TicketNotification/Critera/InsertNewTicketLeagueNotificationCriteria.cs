namespace OWP.TicketNotification.Critera;

public class InsertNewTicketLeagueNotificationCriteria : InsertNotificationMessageCriteria
{
    public int TicketId { get; set; }

    public override string GetSettingKey() => "OWP_TicketCenter_InsertNewTicketLeagueNotification";

    public override bool IsValid()
        => NotificationContent.Length > 0 &&
           TicketId > 0;
}
