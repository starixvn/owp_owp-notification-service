using MediatR;
using System.Globalization;

namespace OWP.Core.NotificationRequests.Prematch;

public class PrematchAssignmentNotifyRequest : NotificationRequest, IRequest<Unit>
{
    public string RedisKey { get; set; }

    public PrematchAssignmentNotifyRequest()
    {
        SessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
    }
}