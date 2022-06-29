using MediatR;
using OWP.NotificationHandler.Services;

namespace OWP.MessageConsumer.Handler;

public class TicketNotificationCommand : NotificationRequest, IRequest<Unit>
{
}

public class NotificationHandler : IRequestHandler<TicketNotificationCommand, Unit>
{
    private readonly NotificationService notifyEngine;

    public NotificationHandler(NotificationService notifyEngine)
    {
        this.notifyEngine = notifyEngine;
    }

    public async Task<Unit> Handle(TicketNotificationCommand request, CancellationToken cancellationToken)
    {
        await notifyEngine.Notify(request.SessionId!);

        return Unit.Value;
    }
}