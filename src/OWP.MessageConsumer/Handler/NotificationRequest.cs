namespace OWP.MessageConsumer.Handler;

public abstract class NotificationRequest
{
    public string? SessionId { get; set; }
}


public class NotificationRequestFactory
{
    public static NotificationRequest GetNotificationRequest(string key, string message)
    {
        return new TicketNotificationCommand
        {
            SessionId = message
        };
    }
}
