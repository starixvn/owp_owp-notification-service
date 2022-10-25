using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewRecipientHandler : IRequestHandler<NewRecipientNotificationRequest, Unit>
{

    private readonly NewRecipientMessageDBWriter newRecipientMessageDBWriter;
    private readonly MessageNotificationService messageNotificationService;
    private readonly NotificationService notificationService;

    public NewRecipientHandler(
        NewRecipientMessageDBWriter newRecipientMessageDBWriter,
        MessageNotificationService messageNotificationService,
        NotificationService notificationService)
    {
        this.newRecipientMessageDBWriter = newRecipientMessageDBWriter;
        this.messageNotificationService = messageNotificationService;
        this.notificationService = notificationService;
    }

    public async Task<Unit> Handle(NewRecipientNotificationRequest request, CancellationToken cancellationToken)
    {
        var messageId = await newRecipientMessageDBWriter.Execute(new NewRecipientModel
        {
            ActorId = request.ActorId,
            RecipientId = request.RecipientId,
            RecipientType = request.RecipientType,
            TicketTitle = request.TicketTitle,
            TicketId = request.TicketId
        });

        var notificationSource = new NotificationSource
        {
            SessionId = messageId,
            TicketId = request.TicketId,
            EventType = request.TicketEventType,
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}
