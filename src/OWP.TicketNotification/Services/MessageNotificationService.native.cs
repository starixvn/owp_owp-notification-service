using OWP.Core.Services;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface MessageNotificationService
{
    Task<List<NotificationModel>> CreateMessageNotification(NotificationSource source);
}

public class MessageNotificationServiceImp(QueryMessageService queryMessageService, TicketRepository ticketRepository)
    : MessageNotificationService
{
    public async Task<List<NotificationModel>> CreateMessageNotification(NotificationSource source)
    {
        var userTuple = await queryMessageService.QueryBySession(source.SessionId);

        var userIds = userTuple.Item1.Select(x => x.UserId).ToList();

        var awaiter = ticketRepository.GetMessageUnreadByMultipleUserId(userIds).ConfigureAwait(true);

        var messageUnread = awaiter.GetAwaiter().GetResult();

        var messages = userTuple.Item2.Select(x => new NotificationModel
        {
            UserId = x.UserId,
            IsImportant = x.IsImportant,
            MessageType = x.MessageType,
            MessageContent = x.MessageContent,
            TicketId = source.TicketId,
            EventType = source.EventType,
            TotalMessageUnread = messageUnread?.FirstOrDefault(m => m.UserId == x.UserId)?.Total,
            IsNewMessage = source.IsNewMessage
        }).DistinctBy(x => new
        {
            x.UserId,
            x.IsImportant,
            x.MessageType,
            x.MessageContent,
            x.TicketId,
            x.EventType,
            x.TotalMessageUnread,
            x.IsNewMessage
        }).ToList();

        return messages;
    }
}