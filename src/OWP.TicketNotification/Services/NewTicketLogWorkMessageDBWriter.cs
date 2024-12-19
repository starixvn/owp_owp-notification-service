using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Criteria;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;


public interface NewTicketLogWorkMessageDBWriter
{
    public Task<string> Execute(NewTicketModel message, NotificationTemplateCode code);
}

public class NewTicketLogWorkMessageDBWriterImp(TicketRepository repository) : NewTicketLogWorkMessageDBWriter
{
    public async Task<string> Execute(NewTicketModel message, NotificationTemplateCode code)
    {
        var template = MessageTemplate.NotificationTemplateMap[(int)code];
        var criteria = new InsertNewTicketLogWorkNotificationCriteria
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

        await repository.InsertNewTicketLogWorkNotification(criteria);

        return criteria.SessionId;
    }
}