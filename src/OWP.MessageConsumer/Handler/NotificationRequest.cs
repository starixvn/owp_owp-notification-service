namespace OWP.MessageConsumer.Handler;

public abstract class NotificationRequest
{
}


public class NotificationRequestFactory
{
    public static NotificationRequest GetNotificationRequest(string key)
    {
        return new TicketNotificationCommand();
    }
}
