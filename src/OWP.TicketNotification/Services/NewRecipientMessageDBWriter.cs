using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Critera;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface NewRecipientMessageDBWriter
{
    Task<string> Execute(NewRecipientModel recipient);
}

public class NewRecipientMessageDBWriterImp : NewRecipientMessageDBWriter
{
    private readonly TicketRepository repository;

    public NewRecipientMessageDBWriterImp(TicketRepository repository)
    {
        this.repository = repository;
    }

    public async Task<string> Execute(NewRecipientModel newRecipient)
    {
        var template = MessageTemplate.NotificationTemplateMap[(int)NotificationTemplateCode.AddRecipient];
        var criteria = new InsertNewRecipientNotificationCriteria
        {
            ActorId = newRecipient.ActorId,
            IsImportant = template.IsImportant,
            IsWatcher = template.IsWatcher,
            ReceivedUserId = newRecipient.RecipientId,
            ReceivedUserType = newRecipient.RecipientType,
            NotificationContent = template.NotificationContent
                .Replace("<ticketId>", $"{newRecipient.TicketId}")
                .Replace("<Event>", newRecipient.TicketTitle),
            NotificationType = template.NotificationType,
        };

        await repository.InsertNewRecipientNotification(criteria);

        return criteria.SessionId;
    }
}