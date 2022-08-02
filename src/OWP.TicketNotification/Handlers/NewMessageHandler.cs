using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewMessageHandler : IRequestHandler<NewMessageNotificationRequest, Unit>
{
    private readonly NewMessageDBWriter writer;
    private readonly MessageNotificationService messageNotificationService;
    private readonly NotificationService notificationService;

    public NewMessageHandler(
        NewMessageDBWriter writer,
        MessageNotificationService messageNotificationService,
        NotificationService notificationService)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
        this.notificationService = notificationService;
    }

    public async Task<Unit> Handle(NewMessageNotificationRequest request, CancellationToken cancellationToken)
    {
        var newMessageModel = new NewMessageModel
        {
            Message = request.Message,
            FileId = request.FileId,
            TicketEventType = request.TicketEventType,
            TicketTitle = request.TicketTitle,
            TicketId = request.TicketId,
            ActorId = request.ActorId
        };
        var messageId = await writer.Execute(newMessageModel);

        var notificationSource = new NotificationSource
        {
            SessionId = messageId,
            TicketId = request.TicketId,
            EventType = request.TicketEventType,
            IsNewMessage = true
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}


