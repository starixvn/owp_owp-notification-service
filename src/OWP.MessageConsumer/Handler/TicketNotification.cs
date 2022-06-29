using MediatR;
using OWP.NotificationHandler.Services;

namespace OWP.MessageConsumer.Handler;

public class TicketNotificationCommand : NotificationRequest, IRequest<Unit>
{
}

public class NotificationHandler : IRequestHandler<TicketNotificationCommand, Unit>
{
    private readonly NotifyEngine notifyEngine;

    public NotificationHandler(NotifyEngine notifyEngine)
    {
        this.notifyEngine = notifyEngine;
    }

    public async Task<Unit> Handle(TicketNotificationCommand request, CancellationToken cancellationToken)
    {
        await notifyEngine.Notify<object>(new List<dynamic> { new { } }, new List<string> { "1" });

        return Unit.Value;
    }
}