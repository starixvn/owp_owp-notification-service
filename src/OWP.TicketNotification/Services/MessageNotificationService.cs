using OWP.Core.Services;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification.Services;

public interface MessageNotificationService
{
    Task<dynamic> CreateMessageNotification(NotificationSource source);
}



public class MessageNotificationServiceImp : MessageNotificationService
{
    private readonly QueryMessageService queryMessageService;
    private readonly TicketRepository ticketRepository;

    public MessageNotificationServiceImp(QueryMessageService queryMessageService, TicketRepository ticketRepository)
    {
        this.queryMessageService = queryMessageService;
        this.ticketRepository = ticketRepository;
    }

    public async Task<dynamic> CreateMessageNotification(NotificationSource source)
    {
        var userTuple = await queryMessageService.QueryBySession(source.SessionId);
        var userIds = userTuple.Item1.Select(x => x.UserId).ToList();
        var awaiter = ticketRepository.GetMessageUnreadByMultipleUserId(userIds).ConfigureAwait(true);
        var messageUnread = awaiter.GetAwaiter().GetResult();
        var messagesDic = userTuple.Item2.Select(x => new NotificationModel
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
        }).ToDictionary(x => x.UserId.ToString(), x => x);

        List<dynamic> messages = new List<dynamic>();

        foreach (var userId in messagesDic.Keys)
        {
            var message = messagesDic[userId];
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message
            };

            messages.Add(result);

        }

        return messages;
    }
}

