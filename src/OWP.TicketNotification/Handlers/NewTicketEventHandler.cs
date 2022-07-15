using MediatR;
using OWP.Core.Contants;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.RabbitMQ.Publisher;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewTicketEventHandler : IRequestHandler<NewTicketEventNotificationRequest, Unit>
{
    private readonly NewTicketMatchMessageDBWriter writer;
    private readonly MessagePublisher publisher;
    private readonly MessageNotificationService messageNotificationService;



    public NewTicketEventHandler(MessagePublisher publisher, NewTicketMatchMessageDBWriter writer, MessageNotificationService messageNotificationService)
    {
        this.publisher = publisher;
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
    }

    public async Task<Unit> Handle(NewTicketEventNotificationRequest request, CancellationToken cancellationToken)
    {
        var notificationMessage = new NewTicketModel
        {
            TicketId = request.TicketId,
            Comment = request.Comment,
            ActorId = request.ActorId
        };
        var messageId = await writer.Execute(notificationMessage, NotificationTemplateCode.NewTicket);

        var notificationSource = new NotificationSource
        {
            SessionId = messageId,
            TicketId = request.TicketId,
            EventType = (int)EventType.Match,
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource); // get mess

        // publish
        publisher.PushMessage(messages, routingKey: RoutingKey.TicketCenterNewTicketEvent);

        return Unit.Value;
    }
}
