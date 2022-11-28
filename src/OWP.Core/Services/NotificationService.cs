using Microsoft.AspNetCore.SignalR;
using OWP.Hubs;
using OWP.Core.Models;
using Newtonsoft.Json;
using Fanex.Logging;

namespace OWP.Core.Services;

public interface NotificationService
{
    Task Notify<T>(IList<MessageModelCommon<T>> sources) where T : class;
}

public class NotificationServiceImp : NotificationService
{
    private readonly IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext;

    private readonly ILogger _logger;

    public NotificationServiceImp(IHubContext<ChangedInfoNotifyHubImp, ChangedInfoNotifyHub> hubContext, ILogger logger)
    {
        this.hubContext = hubContext;
        _logger = logger;
    }

    public async Task Notify<T>(IList<MessageModelCommon<T>> sources) where T : class
    {
        _logger.Info(JsonConvert.SerializeObject(sources.Select(x => x.Message)));
        _logger.Info(JsonConvert.SerializeObject(sources.Select(x => x.ReceivedUserIds)));
        foreach (var message in sources)
        {
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message = message.Message
            };
            await hubContext.Clients
                .All
                .notifyChangedInformationData(result);
        }
    }
}

