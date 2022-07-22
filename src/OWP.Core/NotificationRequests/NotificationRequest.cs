using MediatR;

namespace OWP.Core.NotificationRequests;

public class NotificationRequest : IRequest<string>
{
    public string SessionId { get; set; }
}
