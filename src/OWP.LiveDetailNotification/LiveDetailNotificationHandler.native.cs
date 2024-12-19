namespace OWP.LiveDetailNotification;

using Core.NotificationRequests.LiveDetail;
using MediatR;
using Services;

public class LiveDetailNotificationHandler(
    LiveDetailMessageNotificationService liveDetailMessage,
    NotificationService notificationService)
    : IRequestHandler<LiveDetailNotifyRequest, Unit>
{
    public async Task<Unit> Handle(LiveDetailNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await liveDetailMessage.HandleMessage(request.SessionId);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}