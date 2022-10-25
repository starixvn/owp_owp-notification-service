using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Criteria;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NewTicketLeagueMessageDBWriter
{
    public Task<string> Execute(NewTicketModel message, NotificationTemplateCode code);
}

public class NewTicketLeagueMessageDBWriterImp : NewTicketLeagueMessageDBWriter
{
    private readonly TicketRepository repository;

    public NewTicketLeagueMessageDBWriterImp(TicketRepository repository)
    {
        this.repository = repository;
    }

    public async Task<string> Execute(NewTicketModel message, NotificationTemplateCode code)
    {
        var template = MessageTemplate.NotificationTemplateMap[(int)code];
        var criteria = new InsertNewTicketLeagueNotificationCriteria
        {
            ActorId = message.ActorId,
            IsImportant = template.IsImportant,
            IsWatcher = template.IsWatcher,
            NotificationContent = template.NotificationContent
                .Replace("<ticketId>", message.TicketId.ToString())
                .Replace("<comment>", message.Comment),
            NotificationType = template.NotificationType,
            TicketId = message.TicketId
        };

        await repository.InsertNewTicketLeagueNotification(criteria);

        return criteria.SessionId;
    }
}