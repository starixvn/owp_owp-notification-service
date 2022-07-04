using Fanex.Data.Repository;
using Microsoft.AspNetCore.SignalR;
using OWP.Core.Hubs;
using OWP.Core.Models;

namespace OWP.Core.Services;

public interface NotificationService
{
    Task Notify(string messages);
    Task Notify(object messages);
    Task Notify<T>(IList<T> messages, IList<string> usersId) where T : MessageModel;
    Task Notify<T>(string sessionId, Func<Tuple<IList<User>, IList<MessageModel>>, IDictionary<string, T>> messageModelFunc) where T : MessageModel;

}

public class NotifyEngineImp : NotificationService
{
    private readonly IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> notificationHub;
    private readonly QueryMessageService messageService;

    public NotifyEngineImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> notificationHub, IDynamicRepository repository, QueryMessageService messageService)
    {
        this.notificationHub = notificationHub;
        this.messageService = messageService;
    }


    public async Task Notify(string messages)
    {
        await notificationHub.Clients.All.notifyChangedInformationData(messages);
    }

    public async Task Notify(object messages)
    {
        await notificationHub.Clients.All.notifyChangedInformationData(messages);
    }

    public async Task Notify<T>(IList<T> messages, IList<string> usersId) where T : MessageModel
    {
        foreach (var message in messages)
        {
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message
            };

            await notificationHub.Clients
                .Users((IReadOnlyList<string>)usersId)
                .notifyChangedInformationData(result);

        }
    }

    public async Task Notify<T>(string sessionId, Func<Tuple<IList<User>, IList<MessageModel>>, IDictionary<string, T>> messageModelFunc) where T : MessageModel
    {
        var userTuple = await messageService.QueryBySession(sessionId);

        var messageModel = messageModelFunc(userTuple);

        await Notify(messageModel);
    }

    private async Task Notify<T>(IDictionary<string, T> messagesKeyValuePair) where T : class
    {
        foreach (var userId in messagesKeyValuePair.Keys)
        {
            var message = messagesKeyValuePair[userId];
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message
            };

            await notificationHub.Clients
                .Users(userId)
                .notifyChangedInformationData(result);
        }
    }
}