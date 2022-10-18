using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers;

public class NewTicketLiveIncidentHandler: IRequestHandler<NewTicketLiveIncidentNotificationRequest, Unit>
{
    private readonly NewTicketLiveIncidentMessageDbWriter writer;
    private readonly MessageNotificationService messageNotificationService;
    private readonly NotificationService notificationService;


    public NewTicketLiveIncidentHandler(
        NewTicketLiveIncidentMessageDbWriter writer,
        MessageNotificationService messageNotificationService,
        NotificationService notificationService)
    {
        this.writer = writer;
        this.messageNotificationService = messageNotificationService;
        this.notificationService = notificationService;
    }

    public async Task<Unit> Handle(NewTicketLiveIncidentNotificationRequest request, CancellationToken cancellationToken)
    {
        var notificationMessage = new NewTicketLiveIncidentModel
        {
            TicketId = request.TicketId,
            Comment = request.Comment,
            ActorId = request.ActorId,
            OwpLeagueName = request.OwpLeagueName,
            OwpHomeName = request.OwpHomeName,
            OwpAwayName = request.OwpAwayName
        };
        var messageId = await writer.Execute(notificationMessage, NotificationTemplateCode.NewTicket);

        var notificationSource = new NotificationSource
        {
            SessionId = messageId,
            TicketId = request.TicketId,
            EventType = (int)EventType.LiveIncident,
        };
        var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

        await notificationService.Notify(messages);

        return Unit.Value;
    }
}
