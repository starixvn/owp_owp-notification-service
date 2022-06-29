using Fanex.Data.Repository;
using MediatR;
using OWP.NotificationHandler.Services;

namespace OWP.MessageConsumer.Handler;

public class TicketNotificationCommand : NotificationRequest, IRequest<Unit>
{
}

public class NotificationHandler : IRequestHandler<TicketNotificationCommand, Unit>
{
    private readonly NotificationService notifyEngine;
    private readonly IDynamicRepository repository;

    public NotificationHandler(NotificationService notifyEngine, IDynamicRepository repository)
    {
        this.notifyEngine = notifyEngine;
        this.repository = repository;
    }

    public async Task<Unit> Handle(TicketNotificationCommand request, CancellationToken cancellationToken)
    {
        //await notifyEngine.Notify(request.SessionId!);

        var data = await notifyEngine.QueryBySession(request.SessionId);

        return Unit.Value;
    }
}