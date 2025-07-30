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
    MessageToRc messageToRc,
    OtherSportRepository repository,
    NotificationService notificationService,
    IncludeBBSectionService bBSectionService,
    CacheManipulator cacheManipulator)
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

        await SendMessageUnlinkEvent(request.SessionId, request.FromDate, request.ToDate);

        return Unit.Value;
    }

    private async Task SendMessageUnlinkEvent(
        string sessionId,
        DateTime fromDate,
        DateTime toDate)
    {
        var newSnapshot = await repository.Query(fromDate, toDate);
        var matches = newSnapshot.Matches.Values;
        var newMatchIds = matches
            .Where(x => x.OfferLive == "Live" && x.AdminsiteMatchId == null)
            .Select(x => x.OSMatchId)
            .Distinct()
            .ToList();

        var eventDates = new List<DateTime>();
        var diffDays = toDate - fromDate;

        for (var i = 0; i <= diffDays.TotalDays; i++)
        {
            eventDates.Add(fromDate.AddDays(i));
        }

        foreach (var eventDate in eventDates)
        {
            await ProcessUnlinkNotificationForDate(eventDate, newMatchIds, matches.ToList());
        }


    }

    private async Task ProcessUnlinkNotificationForDate(DateTime eventDate, List<int> newMatchIds, List<MatchInfo> assignments)
    {
        var previouslyNotifiedMatches = await cacheManipulator.GetNotificatedMatchesUnlink(eventDate);
        var matchesToUnlink = newMatchIds.Except(previouslyNotifiedMatches).ToList();

        SendUnlinkNotification(eventDate, matchesToUnlink, assignments);

        // Combine previously notified matches with new matches to unlink
        var updatedNotifiedMatches = previouslyNotifiedMatches.Union(matchesToUnlink).ToList();
        if (updatedNotifiedMatches.Count != previouslyNotifiedMatches.Count)
        {
            await cacheManipulator.SaveNotificatedMatchesUnlink(eventDate, updatedNotifiedMatches);
        }
    }


    private void SendUnlinkNotification(DateTime eventDate, List<int> matchesToUnlink, List<MatchInfo> assignments)
    {
        var assignmentsToNotify = assignments.Where(x => matchesToUnlink.Contains(x.OSMatchId)).ToList();

        // Sort by sport (Basketball, Tennis, Other Sport) and then by earliest kick-off time
        var sortedAssignments = assignmentsToNotify
            .OrderBy(x => GetSportPriority(x.Sport))
            .ThenBy(x => x.DateTime)
            .ToList();

        var unlinkNotificationList = new List<UnlinkNotificationList>();

        foreach (var assignment in sortedAssignments)
        {
            var notificationMessage = CreateMatchNotificationMessage(assignment);
            unlinkNotificationList.Add(new UnlinkNotificationList
            {
                OsMatchId = assignment.OSMatchId,
                Message = notificationMessage
            });
        }

        SendNotification(unlinkNotificationList);
    }

    private int GetSportPriority(string sport)
    {
        return sport?.ToLower() switch
        {
            "basketball" => 1,
            "tennis" => 2,
            _ => 3 // Other Sport
        };
    }

    private string CreateMatchNotificationMessage(MatchInfo assignment)
    {
        return $"{assignment.Sport}\nK.O Time: {assignment.DateTime.ToString("yyyy-MM-dd HH:mm")}\nLeague: {assignment.League}\nMatch: {assignment.Home} - {assignment.Away}";
    }
    
    private void SendNotification(List<UnlinkNotificationList> unlinkNotificationList)
    {
        foreach (var notification in unlinkNotificationList)
        {
            Console.WriteLine($"\n{notification.Message}\n--------------------------------");
        }
    }



}
