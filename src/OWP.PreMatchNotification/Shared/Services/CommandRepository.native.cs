using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.PreMatchNotification.Criteria;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Shared.Services;

public interface CommandRepository
{
    Task InsertMessage(List<MessageModel> messages, string sessionId, int ActorId = 0);

    Task<IList<Assignee>> GetEmployeesBySection(string section);

    Task<IList<Assignee>> GetPreMatchLeagueAssignees(IList<int> preMatchLeagueIds);

    Task<IList<Assignee>> GetPreMatchLeagueAssigneesByOWPLeagueIds(IList<int> owpLeagueIds, DateTime eventDate);
}

public class CommandRepositoryImp : CommandRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public CommandRepositoryImp(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

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
}