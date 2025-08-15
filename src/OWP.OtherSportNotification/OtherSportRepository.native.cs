using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.OtherSportNotification.Criteria;
using OWP.OtherSportNotification.Models;
using System;
using Olympus.OWP.Log.Data.Repository.Criteria;

namespace OWP.OtherSportNotification;

public interface OtherSportRepository
{
    Task InsertMessage(List<MessageModel> messages, string sessionId, int createdBy);
    Task<AssignmentSnapshot> Query(DateTime fromDate, DateTime toDate);
    Task<HashSet<string>> GetUnsubscribeSettings();
   
    Task<IList<string>> QuerySectionBBSupportive();

    Task WriteLog(OwpLogCriteria owpLogCriteria);
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

    public async Task<IList<string>> QuerySectionBBSupportive()
    => (await repository.FetchAsync<string>(new GetBBSupportiveSectionCommand())).ToList();

    public async Task WriteLog(OwpLogCriteria owpLogCriteria)
    {
        await repository.ExecuteAsync(owpLogCriteria);
    }
}
