using Fanex.Data.Repository;
using OWP.PreMatchNotification.LinkedMatch.Criteria;
using OWP.PreMatchNotification.LinkedMatch.Models;

namespace OWP.PreMatchNotification.LinkedMatch.Services;

public interface Repository
{
    Task<List<GeneralMatch>> GetLGAMatchNewSnapshot(DateTime eventDate);

    Task<List<InfositePrematchLeague>> GetInfositePrematch(DateTime eventDate);
}

public class RepositoryImp(IDynamicRepository dynamicRepository) : Repository
{
    public async Task<List<GeneralMatch>> GetLGAMatchNewSnapshot(DateTime eventDate)
    {
        var criteria = new GetLGAMatchesByTimeRangeCriteria
        {
            EventDate = eventDate
        };
        var result = await dynamicRepository.FetchAsync<GeneralMatch>(criteria);

        return result.ToList();
    }

    public async Task<List<InfositePrematchLeague>> GetInfositePrematch(DateTime eventDate)
    {
        var criteria = new GetInfositePrematchCriteria
        {
            EventToDateTime = eventDate.AddDays(1).AddHours(11).AddMinutes(59).AddSeconds(59),
            EventFromDateTime = eventDate.AddHours(12)
        };

        var result = await dynamicRepository.FetchAsync<InfositePrematchLeague>(criteria);

        return result.ToList();
    }
}