using MediatR;

namespace OWP.Core.NotificationRequests;

public class OtherSportNotificationRequest : NotificationRequest, IRequest<Unit>
{
    public string EventDate { get; internal set; }
}
