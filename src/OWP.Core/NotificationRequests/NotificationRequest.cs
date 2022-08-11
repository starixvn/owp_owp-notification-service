using MediatR;

namespace OWP.Core.NotificationRequests;

public class NotificationRequest : IRequest<Unit>
{
    public string SessionId { get; set; }
}
