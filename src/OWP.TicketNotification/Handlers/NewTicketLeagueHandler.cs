using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.MessageConsumer.Constants;
using OWP.MessagePublisher;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewTicketLeagueHandler : IRequestHandler<NewTicketLeagueNotificationRequest, Unit>
{
    private readonly NewTicketLeagueMessageDBWriter writer;
    private readonly Publisher publisher;
    private readonly MessageNotificationService messageNotificationService;

    public NewTicketLeagueHandler(NewTicketLeagueMessageDBWriter writer, Publisher publisher, MessageNotificationService messageNotificationService)
    {
        this.writer = writer;
        this.publisher = publisher;
        this.messageNotificationService = messageNotificationService;
    }


    public async Task<Unit> Handle(NewTicketLeagueNotificationRequest request, CancellationToken cancellationToken)
    {
        var notificationMessage = new NewTicketModel
        {
            TicketId = request.TicketId,
            Comment = request.Comment,
            ActorId = request.CreatorId
        };
        var messageId = await writer.Execute(notificationMessage, NotificationTemplateCode.NewTicket);

        var notificationSource = new NotificationSource
        {
            SessionId = messageId,
            TicketId = request.TicketId,
            EventType = (int)EventType.League,
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

        // publish
        publisher.PushMessage(messages, routingKey: RoutingKey.TicketCenterNewTicketLeague);

        return Unit.Value;
    }
}
