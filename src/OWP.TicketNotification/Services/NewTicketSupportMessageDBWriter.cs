using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Critera;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NewTicketSupportMessageDBWriter
{
    public Task<string> Execute(NewTicketModel message, NotificationTemplateCode code);
}

public class NewTicketSupportMessageDBWriterImp : NewTicketSupportMessageDBWriter
{
    private readonly TicketRepository repository;

    public NewTicketSupportMessageDBWriterImp(TicketRepository repository)
    {
        this.repository = repository;
    }

    public async Task<string> Execute(NewTicketModel message, NotificationTemplateCode code)
    {

        var template = MessageTemplate.NotificationTemplateMap[(int)code];
        var criteria = new InsertNewTicketSupportNotificationCriteria
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

        await repository.InsertNewTicketSupportNotification(criteria);

        return criteria.SessionId;
    }
}