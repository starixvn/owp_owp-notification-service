using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewRecipientHandler : IRequestHandler<NewRecipientNotificationRequest, List<object>>
{

    private readonly NewRecipientMessageDBWriter newRecipientMessageDBWriter;
    private readonly MessageNotificationService messageNotificationService;

    public NewRecipientHandler(NewRecipientMessageDBWriter newRecipientMessageDBWriter, MessageNotificationService messageNotificationService)
    {
        this.newRecipientMessageDBWriter = newRecipientMessageDBWriter;
        this.messageNotificationService = messageNotificationService;
    }

    public async Task<List<object>> Handle(NewRecipientNotificationRequest request, CancellationToken cancellationToken)
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
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource); // get mess

        var result = messages.Select(m => new
        {
            NotificationId = Guid.NewGuid().ToString(),
            Message = m
        }).ToList();

        return result.ToList<object>();
    }
}
