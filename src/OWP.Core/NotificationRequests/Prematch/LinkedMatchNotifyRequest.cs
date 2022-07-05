using MediatR;

namespace OWP.Core.NotificationRequests.Prematch;

public class LinkedMatchNotifyRequest : NotificationRequest, IRequest<Unit>
{
    public DateTime EventDate { get; set; }
}
