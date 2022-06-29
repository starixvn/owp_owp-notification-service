using System.Globalization;
using MediatR;
using OWP.Core.Models;

namespace OWP.Core.NotificationRequests.Prematch;

public class PrematchAssignmentNotifyRequest : NotificationCommandBaseModel, IRequest<Unit>
{
    public string RedisKey { get; set; }

    public PrematchAssignmentNotifyRequest()
    {
        SessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
    }
}