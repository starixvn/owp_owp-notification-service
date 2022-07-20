using MediatR;

namespace OWP.Core.NotificationRequests.OtherSport;

public class OtherSportNotificationRequest : NotificationRequest, IRequest<List<object>>
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
    public int ActorId { get; set; }
}
