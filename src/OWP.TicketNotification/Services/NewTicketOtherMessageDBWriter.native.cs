using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Criteria;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NewTicketOtherMessageDBWriter
{
    public Task<string> Execute(NewTicketModel message, NotificationTemplateCode code);
}

public class NewTicketOtherMessageDBWriterImp(TicketRepository repository) : NewTicketOtherMessageDBWriter
{
    public async Task<string> Execute(NewTicketModel message, NotificationTemplateCode code)
    {
        var template = MessageTemplate.NotificationTemplateMap[(int)code];
        var criteria = new InsertNewTicketOtherNotificationCriteria
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

        await repository.InsertNewTicketOtherNotification(criteria);

        return criteria.SessionId;
    }
}