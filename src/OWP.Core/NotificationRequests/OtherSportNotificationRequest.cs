using MediatR;

namespace OWP.Core.NotificationRequests;

public class OtherSportNotificationRequest : NotificationRequest, IRequest<Unit>
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
}
