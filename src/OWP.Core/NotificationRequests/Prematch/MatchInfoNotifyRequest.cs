using MediatR;
using OWP.Core.Models;

namespace OWP.Core.NotificationRequests.Prematch;

public class MatchInfoNotifyRequest : NotificationCommandBaseModel, IRequest<Unit>
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
}