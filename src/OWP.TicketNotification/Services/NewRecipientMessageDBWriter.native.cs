using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Criteria;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NewRecipientMessageDBWriter
{
    Task<string> Execute(NewRecipientModel recipient);
}

public class NewRecipientMessageDBWriterImp(TicketRepository repository) : NewRecipientMessageDBWriter
{
    public async Task<string> Execute(NewRecipientModel recipient)
    {
        var template = MessageTemplate.NotificationTemplateMap[(int)NotificationTemplateCode.AddRecipient];
        var criteria = new InsertNewRecipientNotificationCriteria
        {
            ActorId = recipient.ActorId,
            IsImportant = template.IsImportant,
            IsWatcher = template.IsWatcher,
            ReceivedUserId = recipient.RecipientId,
            ReceivedUserType = recipient.RecipientType,
            NotificationContent = template.NotificationContent
                .Replace("<ticketId>", $"{recipient.TicketId}")
                .Replace("<Event>", recipient.TicketTitle),
            NotificationType = template.NotificationType
        };

        await repository.InsertNewRecipientNotification(criteria);

        return criteria.SessionId;
    }
}