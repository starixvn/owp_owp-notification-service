using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.OtherSportNotification.Criteria;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification;

public interface OtherSportRepository
{
    Task InsertMessage(List<MessageModel> messages, string sessionId, int createdBy);
    Task<AssignmentSnapshot> Query(DateTime fromDate, DateTime toDate);
    Task<HashSet<string>> GetUnsubscribeSettings();
    Task<Dictionary<string, List<Assignee>>> GetSessionWatcherSettings();

    Task<IList<string>> QuerySectionBBSupportive();
}

public class OtherSportRepositoryImp(IDynamicRepository repository) : OtherSportRepository
{
    public async Task InsertMessage(List<MessageModel> messages, string sessionId, int createdBy)
    {
        messages.ForEach(x => x.ReceivedUserIdList = string.Join(",", x.ReceivedUserIds));

        var criteria = new InsertMessageCommand
        {
            Messages = XmlConverter.Serialize(messages),
            CreatedByUserId = createdBy,
            SessionId = sessionId
        };

        await repository.ExecuteAsync(criteria);
    }

    public async Task<AssignmentSnapshot> Query(DateTime fromDate, DateTime toDate)
    {
        var criteria = new GetAllMatchesByDateTimeListCriteria
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var (firstResultSet, secondResultSet, thirdResultSet) = await repository.FetchMultipleAsync<MatchInfo, MatchAssignment, MatchSetting>(criteria);

        return new AssignmentSnapshot
        {
            Matches = firstResultSet.ToDictionary(m => m.OSMatchId),
            MatchAssignments = secondResultSet.GroupBy(a => a.OSMatchId).ToDictionary(a => a.Key, a => a.ToList()),
            MatchSettings = thirdResultSet.GroupBy(a => a.OSMatchId).ToDictionary(a => a.Key, a => a.ToList())
        };
    }

    public async Task<HashSet<string>> GetUnsubscribeSettings()
    {
        var criteria = new GetNotificationSettingCommand();
        var notificationSettings = await repository.FetchAsync<NotificationSetting>(criteria);

        return [..notificationSettings.Select(a => $"{a.UserId}-{a.NotificationSettingId}")];
    }

    public async Task<Dictionary<string, List<Assignee>>> GetSessionWatcherSettings()
    {
        var criteria = new GetSessionReceivedWatcherCommand();

        var watchers = (await repository.FetchAsync<SessionWatcher>(criteria)).ToList();

        return watchers.GroupBy(a => $"{a.UserId}-{a.NotificationSettingId}")
                     .ToDictionary(key => key.Key, value => value.Select(i => new Assignee
                     {
                         AssigneeId = i.UserWatcherId,
                         AssigneeName = i.UserWatcherName
                     }).ToList());
    }

    public async Task<IList<string>> QuerySectionBBSupportive()
    => (await repository.FetchAsync<string>(new GetBBSupportiveSectionCommand())).ToList();
}
