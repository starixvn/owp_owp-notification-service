using System.Globalization;

namespace OWP.Core.NotificationRequests.Prematch;

public class PrematchAssignmentNotifyRequest : NotificationRequest
{
    public string RedisKey { get; set; }

    public string ActorId { get; set; }

    public PrematchAssignmentNotifyRequest()
    {
        SessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
    }
}