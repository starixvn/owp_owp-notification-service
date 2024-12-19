using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewRecipientHandler(
    NewRecipientMessageDBWriter newRecipientMessageDbWriter,
    MessageNotificationService messageNotificationService,
    NotificationService notificationService)
    : IRequestHandler<NewRecipientNotificationRequest, Unit>
{
    public async Task<Unit> Handle(NewRecipientNotificationRequest request, CancellationToken cancellationToken)
    {
        var messageId = await newRecipientMessageDbWriter.Execute(new NewRecipientModel
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
            EventType = request.TicketEventType
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}
