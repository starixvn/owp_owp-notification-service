using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.MessageConsumer.Constants;
using OWP.MessagePublisher;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewMessageHandler : IRequestHandler<NewMessageNotificationRequest, Unit>
{
    private readonly NewMessageDBWriter writer;
    private readonly MessageNotificationService messageNotificationService;
    private readonly Publisher publisher;

    public NewMessageHandler(NewMessageDBWriter writer, MessageNotificationService messageNotificationService, Publisher publisher)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
        this.publisher = publisher;
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
        var messageId = await writer.Execute(newMessageModel); // insert messs

        var notificationSource = new NotificationSource
        {
            SessionId = messageId,
            TicketId = request.TicketId,
            EventType = request.TicketEventType,
            IsNewMessage = true
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource); // get mess

        // publish
        publisher.PushMessage(messages, routingKey: RoutingKey.TicketCenterNewMessage);

        return Unit.Value;
    }
}


