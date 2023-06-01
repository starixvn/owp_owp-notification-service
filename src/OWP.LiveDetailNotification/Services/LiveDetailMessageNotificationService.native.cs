namespace OWP.LiveDetailNotification.Services;

using Core.Models;
using Core.Services;

public interface LiveDetailMessageNotificationService
{
    Task<List<MessageModel>> HandleMessage(string sessionId);
}

public class LiveDetailMessageNotificationServiceImp : LiveDetailMessageNotificationService
{
    private readonly QueryMessageService queryMessageService;

    public LiveDetailMessageNotificationServiceImp(QueryMessageService queryMessageService)
    {
        this.queryMessageService = queryMessageService;
    }

    public async Task<List<MessageModel>> HandleMessage(string sessionId)
    {
        var tuple = await queryMessageService.QueryBySession(sessionId);
        var messages = tuple.Item2.Select(x => new MessageModel
        {
            UserId = x.UserId,
            IsImportant = x.IsImportant,
            MessageType = x.MessageType,
            MessageContent = x.MessageContent
        }).ToList();
        return messages;
    }
}