using Fanex.Data.Repository;
using OWP.PreMatchNotification.InfoSiteMatchInfo.Criteria;
using OWP.PreMatchNotification.InfoSiteMatchInfo.Models;

namespace OWP.PreMatchNotification.InfoSiteMatchInfo.Services;

public interface Repository
{
    Task<IList<InfoSiteMatch>> GetInfoSiteMatchNewSnapshot(DateTime fromDate, DateTime toDate);

    Task<IList<InfoSiteMatch>> GetLGALinkedInfoSiteMatch(List<int> infoSiteMatchIds);
}

public class RepositoryImp : Repository
{
    private readonly IDynamicRepository dynamicRepository;

    public RepositoryImp(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<IList<InfoSiteMatch>> GetInfoSiteMatchNewSnapshot(DateTime fromDate, DateTime toDate)
    {
        var criteria = new GetInfoSiteMatchesByDateRangeCriteria
        {
            FromDate = fromDate.Date.AddHours(12),
            ToDate = toDate.AddHours(11).AddMinutes(59)
        };
        var result = await dynamicRepository.FetchAsync<InfoSiteMatch>(criteria);

        return result.ToList();
    }

    public async Task<IList<InfoSiteMatch>> GetLGALinkedInfoSiteMatch(List<int> infoSiteMatchIds)
    {
        var criteria = new GetLinkedOWPMatchCriteria
        {
            InfositeMatchIds = string.Join(',', infoSiteMatchIds)
        };
        var result = await dynamicRepository.FetchAsync<InfoSiteMatch>(criteria);

        return result.ToList();
    }
}