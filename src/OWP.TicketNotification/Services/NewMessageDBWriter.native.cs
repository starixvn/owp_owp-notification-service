
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Critera;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;


public interface NewMessageDBWriter
{
    Task<string> Execute(NewMessageModel newMessage);
}

public class NewMessageDBWriterImp : NewMessageDBWriter
{
    private readonly TicketRepository repository;

    public NewMessageDBWriterImp(TicketRepository repository)
    {
        this.repository = repository;
    }

    public async Task<string> Execute(NewMessageModel newMessage)
    {
        var templateCode = newMessage.FileId.HasValue
            ? NotificationTemplateCode.NewAttachment
            : NotificationTemplateCode.NewMessage;
        var template = MessageTemplate.NotificationTemplateMap[(int)templateCode];

        var criteria = new InsertNewMessageNotificationCriteria
        {
            NotificationContent = template.NotificationContent
                .Replace("<ticketId>", $"{newMessage.TicketId}")
                .Replace("<Event>", newMessage.TicketTitle)
                .Replace("<comment>", newMessage.Message),
            NotificationType = template.NotificationType,
            IsImportant = template.IsImportant,
            IsWatcher = template.IsWatcher,
            TicketId = newMessage.TicketId,
            ActorId = newMessage.ActorId,
        };

        await repository.InsertNewMessageNotification(criteria);

        return criteria.SessionId;
    }
}