using Fanex.Data.Repository;
using System.Globalization;

namespace OWP.TicketNotification.Criteria;

public abstract class InsertNotificationMessageCriteria : NonQueryCommand
{
    public string NotificationContent { get; set; }

    public bool IsImportant { get; set; }

    public bool IsWatcher { get; set; }

    public int NotificationType { get; set; }

    public int ActorId { get; set; }

    public string SessionId { get; } = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
}
