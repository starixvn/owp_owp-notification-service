using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Criteria;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NewTicketLiveIncidentMessageDbWriter
{
    public Task<string> Execute(NewTicketLiveIncidentModel message, NotificationTemplateCode code);
}

public class NewTicketLiveIncidentMessageDbWriterImp: NewTicketLiveIncidentMessageDbWriter
{
    private readonly TicketRepository repository;

    public NewTicketLiveIncidentMessageDbWriterImp(TicketRepository repository)
    {
        this.repository = repository;
    }
    public async Task<string> Execute(NewTicketLiveIncidentModel message, NotificationTemplateCode code)
    {
        var template = MessageTemplate.NotificationTemplateMap[(int)code];
        var criteria = new InsertNewTicketLiveIncidentNotificationCriteria
        {
            ActorId = message.ActorId,
            IsImportant = template.IsImportant,
            IsWatcher = template.IsWatcher,
            NotificationContent = template.NotificationContent
                .Replace("<ticketId>", message.TicketId.ToString())
                .Replace("<comment>", message.Comment),
            NotificationType = template.NotificationType,
            TicketId = message.TicketId,
            OwpLeagueName = message.OwpLeagueName,
            OwpHomeName = message.OwpHomeName,
            OwpAwayName = message.OwpAwayName
        };

        await repository.InsertNewTicketLiveIncidentNotification(criteria);

        return criteria.SessionId;
    }
}
