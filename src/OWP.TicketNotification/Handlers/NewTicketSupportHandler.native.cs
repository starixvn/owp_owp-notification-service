using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

internal class NewTicketSupportHandler(
    NewTicketSupportMessageDBWriter writer,
    MessageNotificationService messageNotificationService,
    NotificationService notificationService)
    : IRequestHandler<NewTicketSupportNotificationRequest, Unit>
{
    public async Task<Unit> Handle(NewTicketSupportNotificationRequest request, CancellationToken cancellationToken)
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
            EventType = (int)EventType.Support
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}