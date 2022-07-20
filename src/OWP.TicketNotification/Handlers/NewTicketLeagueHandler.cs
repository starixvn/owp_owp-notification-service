using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewTicketLeagueHandler : IRequestHandler<NewTicketLeagueNotificationRequest, List<object>>
{
    private readonly NewTicketLeagueMessageDBWriter writer;
    private readonly MessageNotificationService messageNotificationService;

    public NewTicketLeagueHandler(NewTicketLeagueMessageDBWriter writer, MessageNotificationService messageNotificationService)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
    }


    public async Task<List<object>> Handle(NewTicketLeagueNotificationRequest request, CancellationToken cancellationToken)
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


        var result = messages.Select(m => new
        {
            NotificationId = Guid.NewGuid().ToString(),
            Message = m
        }).ToList();

        return result.ToList<object>();
    }
}
