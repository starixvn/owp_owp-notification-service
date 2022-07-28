using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewMessageHandler : IRequestHandler<NewMessageNotificationRequest, string>
{
    private readonly NewMessageDBWriter writer;
    private readonly MessageNotificationService messageNotificationService;

    public NewMessageHandler(NewMessageDBWriter writer, MessageNotificationService messageNotificationService)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
    }

    public async Task<string> Handle(NewMessageNotificationRequest request, CancellationToken cancellationToken)
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
        var messageId = await writer.Execute(newMessageModel); // insert mess

        var notificationSource = new NotificationSource
        {
            SessionId = messageId,
            TicketId = request.TicketId,
            EventType = request.TicketEventType,
            IsNewMessage = true
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource); // get mess

        return JsonConvert.SerializeObject(messages);
    }
}


