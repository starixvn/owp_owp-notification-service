namespace OWP.Core.NotificationRequests;

public class NotificationRequestFactory
{
    public static NotificationRequest GetNotificationRequest(string key, object message)
    {
        return new NewTicketNotificationRequest();
    }
}
