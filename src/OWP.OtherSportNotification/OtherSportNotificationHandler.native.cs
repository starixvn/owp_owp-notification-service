using System.Globalization;
using MediatR;
using Microsoft.Extensions.Configuration;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Models;
using OWP.OtherSportNotification.Services;
using System.Text.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using System.Net.Http.Json;
using Fanex.Logging;
using Fanex.Caching;
using Newtonsoft.Json;

namespace OWP.OtherSportNotification;

public class RocketChatAuthResponse
{
    public string Status { get; set; }
    public AuthData Data { get; set; }

    public class AuthData
    {
        public string AuthToken { get; set; }
        public string UserId { get; set; }
    }
}

public class RCResponse
{
    public bool Success { get; set; }
    public List<string> Message { get; set; } = [];
}

public class LastUpdateInfo
{
    public DateTime Date { get; set; }

    public string Username { get; set; }
}

public class OtherSportNotificationHandler(
    ChangedValueByCacheAndDatabase changedValueByCacheAndDatabase,
    AssigneeChangedValueFilter assigneeChangedValueFilter,
    ChangedValueAggregator changedValueAggregator,
    MessageFactory messageFactory,
    MessageToRocketChat messageToRc,
    OtherSportRepository repository,
    NotificationService notificationService,
    IncludeBBSectionService bBSectionService,
    RocketChatNotifyEventUnlink notifyEventUnlink)
    : IRequestHandler<OtherSportNotificationRequest, Unit>
{

    public async Task<Unit> Handle(OtherSportNotificationRequest request, CancellationToken cancellationToken)
    {
        var messages = await changedValueByCacheAndDatabase.Create(request.SessionId, request.FromDate, request.ToDate);
        var assigneeChangedValues = await assigneeChangedValueFilter.Filter(messages);
        var aggregatedChangedValues = changedValueAggregator.Aggregate(assigneeChangedValues);

        var messageModels = messageFactory.Transform(aggregatedChangedValues, request.UserName);

        await repository.InsertMessage(messageModels, request.SessionId, request.ActorId);

        await notificationService.Notify(messageModels);

        await bBSectionService.InsertMessage(messages, request);
        await messageToRc.Send(aggregatedChangedValues, request);
        
        return Unit.Value;
    }

    



}
