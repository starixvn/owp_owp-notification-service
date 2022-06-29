using MediatR;
using OWP.Core.Models;

namespace OWP.Core.NotificationRequests.Prematch;

public class LinkedMatchNotifyRequest : NotificationCommandBaseModel, IRequest<Unit>
{
    public DateTime EventDate { get; set; }
}
