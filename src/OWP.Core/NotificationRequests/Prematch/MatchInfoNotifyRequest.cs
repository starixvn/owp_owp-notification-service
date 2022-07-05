using MediatR;

namespace OWP.Core.NotificationRequests.Prematch;

public class MatchInfoNotifyRequest : NotificationRequest, IRequest<Unit>
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
}