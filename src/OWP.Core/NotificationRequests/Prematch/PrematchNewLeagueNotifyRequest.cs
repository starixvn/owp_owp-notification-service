using MediatR;
using OWP.Core.Models;

namespace OWP.Core.NotificationRequests.Prematch;

public class PrematchNewLeagueNotifyRequest : NotificationCommandBaseModel, IRequest<Unit>
{
    public DateTime FromDate { get; set; }
}