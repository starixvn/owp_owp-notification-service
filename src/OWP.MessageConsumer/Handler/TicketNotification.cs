using MediatR;

namespace OWP.MessageConsumer.Handler;

public class TicketNotificationCommand : NotificationRequest, IRequest<Unit>
{
}

public class NotificationHandler : IRequestHandler<TicketNotificationCommand, Unit>
{
    public Task<Unit> Handle(TicketNotificationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}