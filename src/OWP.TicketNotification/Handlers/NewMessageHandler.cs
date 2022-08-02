using Fanex.Logging;
using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewMessageHandler : IRequestHandler<NewMessageNotificationRequest, Unit>
{
    private readonly NewMessageDBWriter writer;
    private readonly MessageNotificationService messageNotificationService;
    private readonly NotificationService notificationService;
    private readonly ILogger logger;

    public NewMessageHandler(
        NewMessageDBWriter writer,
        MessageNotificationService messageNotificationService,
        NotificationService notificationService,
        ILogger logger)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
        this.notificationService = notificationService;
        this.logger = logger;
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

        logger.Info(JsonConvert.SerializeObject(messages));
        await notificationService.Notify(messages);

        return Unit.Value;
    }
}


