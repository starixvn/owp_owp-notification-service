namespace OWP.LiveDetailNotification.Services;

using Core.Models;
using Core.Services;

public interface LiveDetailMessageNotificationService
{
    Task<List<MessageModel>> HandleMessage(string sessionId);
}

public class LiveDetailMessageNotificationServiceImp(QueryMessageService queryMessageService)
    : LiveDetailMessageNotificationService
{
    public async Task<List<MessageModel>> HandleMessage(string sessionId)
    {
        var tuple = await queryMessageService.QueryBySession(sessionId);
       
        var messages = tuple.Item2.Select(x => new MessageModel
        {
            UserId = x.UserId,
            IsImportant = x.IsImportant,
            MessageType = x.MessageType,
            MessageContent = x.MessageContent,
            Recipient = tuple.Item1.First(u=>u.UserId==x.UserId).Username
        }).ToList();
        return messages;
    }
}