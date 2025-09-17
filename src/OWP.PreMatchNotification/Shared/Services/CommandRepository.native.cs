using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.PreMatchNotification.NewLeague.Criteria;
using OWP.PreMatchNotification.NewLeague.Models;
using OWP.PreMatchNotification.Shared.Criteria;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Shared.Services;

public interface CommandRepository
{
    Task InsertMessage(List<MessageModel> messages, string sessionId, int ActorId = 0);

    Task<IList<Assignee>> GetEmployeesBySection(string section);

    Task<IList<Assignee>> GetPreMatchLeagueAssignees(IList<int> preMatchLeagueIds);

    Task<IList<Assignee>> GetPreMatchLeagueAssigneesByOWPLeagueIds(IList<int> owpLeagueIds, DateTime eventDate);

    Task<TimeSpan?> QueryExistedStartTime(DateTime eventDate);

    Task<List<string>> QueryDayShiftPreMatchAssignments(QueryShiftPreMatchCriteria criteria);

}

public class CommandRepositoryImp(IDynamicRepository dynamicRepository) : CommandRepository
{
    public async Task InsertMessage(List<MessageModel> messages, string sessionId, int ActorId = 0)
    {
        messages.ForEach(x => x.ReceivedUserIdList = string.Join(",", x.ReceivedUserIds));

        var criteria = new InsertMessageCommand
        {
            Messages = XmlConverter.Serialize(messages),
            CreatedByUserId = ActorId,
            SessionId = sessionId
        };

        await dynamicRepository.ExecuteAsync(criteria);
    }

    public async Task<IList<Assignee>> GetEmployeesBySection(string section)
    {
        var criteria = new QuerySectionEmployeeCriteria
        {
            Section = section
        };
        var result = await dynamicRepository.FetchAsync<Assignee>(criteria).ConfigureAwait(false);

        return result.ToList();
    }

    public async Task<IList<Assignee>> GetPreMatchLeagueAssignees(IList<int> preMatchLeagueIds)
    {
        var criteria = new QueryLeagueAssigneeCriteria
        {
            PreMatchLeagueIds = string.Join(',', preMatchLeagueIds)
        };
        var result = await dynamicRepository.FetchAsync<Assignee>(criteria).ConfigureAwait(false);

        return result.ToList();
    }

    public async Task<IList<Assignee>> GetPreMatchLeagueAssigneesByOWPLeagueIds(IList<int> owpLeagueIds, DateTime eventDate)
    {
        var criteria = new QueryLeagueAssigneeCriteriaByOWPLeagueIds
        {
            OWPLeagueIds = string.Join(',', owpLeagueIds),
            EventDate = eventDate
        };
        var result = await dynamicRepository.FetchAsync<Assignee>(criteria).ConfigureAwait(false);

        return result.ToList();
    }

    public async Task<List<string>> QueryDayShiftPreMatchAssignments(QueryShiftPreMatchCriteria criteria)
    {
        try
        {

        return (await dynamicRepository.FetchAsync<PrematchLeagueInfo>(criteria)).Select(x => x.LeagueName).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<TimeSpan?> QueryExistedStartTime(DateTime eventDate)
    {
        var criteria = new GetStartTimeCriteria
        {
            EventDate = eventDate
        };
        var result = (await dynamicRepository.FetchAsync<TimeSpan>(criteria)).ToList();

        return result.Any() ? result.FirstOrDefault() : null;
    }
}