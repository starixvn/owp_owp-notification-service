using Fanex.Data.Repository;
using Microsoft.AspNetCore.SignalR;
using OWP.Core.Criteria;
using OWP.Core.Models;
using OWP.NotificationHandler.Hubs;

namespace OWP.NotificationHandler.Services;

public interface NotifyEngine
{
    Task Notify<T>(IList<T> messages, IList<string> usersId) where T : class;

    Task Notify<T>(string sessionId, Func<Tuple<IList<User>, IList<MessageModel>>, IDictionary<string, T>> messageModelFunc) where T : class;
}

public class NotifyEngineImp : NotifyEngine
{
    private readonly IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> notificationHub;
    private readonly IDynamicRepository repository;

    public NotifyEngineImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> notificationHub)
    {
        this.notificationHub = notificationHub;
    }

    public async Task Notify<T>(IList<T> messages, IList<string> usersId) where T : class
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

    public async Task Notify<T>(string sessionId, Func<Tuple<IList<User>, IList<MessageModel>>, IDictionary<string, T>> messageModelFunc) where T : class
    {
        var userTuple = await QueryBySession(sessionId);

        var messageModel = messageModelFunc(userTuple);

        await Notify(messageModel);
    }

    private async Task<Tuple<IList<User>, IList<MessageModel>>> QueryBySession(string sessionId)
    {
        var criteria = new GetMessageBySessionCriteria
        {
            SessionId = sessionId
        };

        var result = await repository.FetchMultipleAsync<User, MessageModel>(criteria);
        return new Tuple<IList<User>, IList<MessageModel>>(result.Item1.ToList(), result.Item2.ToList());
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