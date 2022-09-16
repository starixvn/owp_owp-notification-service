namespace OWP.LiveDetailNotification;

using Core.NotificationRequests.LiveDetail;
using MediatR;
using Services;

public class LiveDetailNotificationHandler : IRequestHandler<LiveDetailNotifyRequest, Unit>
{
    private readonly LiveDetailMessageNotificationService liveDetailMessage;

    private readonly NotificationService notificationService;

    public LiveDetailNotificationHandler(LiveDetailMessageNotificationService liveDetailMessage,
        NotificationService notificationService)
    {
        this.liveDetailMessage = liveDetailMessage;
        this.notificationService = notificationService;
    }

    public async Task<Unit> Handle(LiveDetailNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await liveDetailMessage.HandleMessage(request.SessionId);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}