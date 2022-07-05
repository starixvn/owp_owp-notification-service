using MediatR;

namespace OWP.Core.NotificationRequests.Prematch;

public class PrematchNewLeagueNotifyRequest : NotificationRequest, IRequest<Unit>
{
    public DateTime FromDate { get; set; }
}