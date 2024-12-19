using Microsoft.AspNetCore.SignalR;
using OWP.Hubs;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Shared.Services;

public interface NotificationService
{
    Task Notify(List<MessageModel> sources);

    Task NotifyGroupMessageForLinkMatch(List<MessageModel> sources);
}

public class NotificationServiceImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext)
    : NotificationService
{
    public async Task Notify(List<MessageModel> sources)
    {
        foreach (var message in sources)
        {
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message
            };
            await hubContext.Clients
#if DEBUG
                .All
#else
                .Users(message.ReceivedUserIds)
#endif
                .notifyChangedInformationData(result);
        }
    }

    public async Task NotifyGroupMessageForLinkMatch(List<MessageModel> sources)
    {
        var groupedMessagesByUser = sources
            .SelectMany(m => m.ReceivedUserIds.Select(userId => new { UserId = userId, Message = m }))
            .GroupBy(x => new { x.UserId, x.Message.MessageType }, x => x.Message)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var message in groupedMessagesByUser)
        {
            var newMessage = GetGroupMessage(message.Value);

            var result = new { notificationId = Guid.NewGuid(), message = newMessage };

            await hubContext.Clients
#if DEBUG
                .All
#else
                .User(message.Key.UserId)
#endif
                .notifyChangedInformationData(result);
        }
    }

    private static MessageModel GetGroupMessage(List<MessageModel> messages)
    {
        var firstMessage = messages.FirstOrDefault();
        var newMessage = new MessageModel
        {
            MessageType = firstMessage.MessageType,
            MessageContent = firstMessage.MessageContent,
            IsBlinking = firstMessage.IsBlinking,
            IsImportant = firstMessage.IsImportant,
            IsWatcher = firstMessage.IsWatcher
        };
        var messageCount = messages.Count;

        var groupMessageDict = new Dictionary<int, string>
        {
            { (int)MsgTemplateCode.LGAAddLiveMatch, "ADD LIVE" },
            { (int)MsgTemplateCode.LGACancelLiveMatch, "CANCEL LIVE" },
            { (int)MsgTemplateCode.LGAChangeMatchStatus, "MATCH STATUS changed" }
        };

        var message = groupMessageDict.GetValueOrDefault(newMessage.MessageType, null);
        if (messageCount == 1 || string.IsNullOrEmpty(message))
        {
            return newMessage;
        }

        newMessage.MessageContent = $"{message} in {messageCount} matches";

        return newMessage;
    }
}
