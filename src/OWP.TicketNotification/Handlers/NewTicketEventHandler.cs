using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewTicketEventHandler : IRequestHandler<NewTicketEventNotificationRequest, string>
{
    private readonly NewTicketMatchMessageDBWriter writer;
    private readonly MessageNotificationService messageNotificationService;

    public NewTicketEventHandler(NewTicketMatchMessageDBWriter writer, MessageNotificationService messageNotificationService)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
    }

    public async Task<string> Handle(NewTicketEventNotificationRequest request, CancellationToken cancellationToken)
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

        return JsonConvert.SerializeObject(messages);
    }
}
