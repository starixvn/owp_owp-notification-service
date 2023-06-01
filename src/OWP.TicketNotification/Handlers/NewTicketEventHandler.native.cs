using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewTicketEventHandler : IRequestHandler<NewTicketEventNotificationRequest, Unit>
{
    private readonly NewTicketMatchMessageDBWriter writer;
    private readonly MessageNotificationService messageNotificationService;
    private readonly NotificationService notificationService;


    public NewTicketEventHandler(
        NewTicketMatchMessageDBWriter writer,
        MessageNotificationService messageNotificationService,
        NotificationService notificationService)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
        this.notificationService = notificationService;
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
            EventType = (int)EventType.Match
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}
