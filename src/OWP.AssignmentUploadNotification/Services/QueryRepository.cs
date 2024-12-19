using Fanex.Data.Repository;
using OWP.AssignmentUploadNotification.Criteria;
using OWP.AssignmentUploadNotification.Models;

namespace OWP.AssignmentUploadNotification.Services;

public interface QueryRepository
{
    Task<AssignmentSnapshot> Query(DateTime eventDate);
}

public class QueryRepositoryImp(IDynamicRepository repository) : QueryRepository
{
    public async Task<AssignmentSnapshot> Query(DateTime eventDate)
    {
        var matchesTask = repository.FetchAsync<GeneralMatch>(new GetAllMatchesByTimeRangeCriteria { EventDate = eventDate });
        var assignmentsTask = repository.FetchAsync<MatchAssignment>(new GetAllAssignmentsByTimeRangeCriteria { EventDate = eventDate });
        var matchSettingsTask = repository.FetchAsync<MatchSetting>(new GetAllMatchSettingsByTimeRangeCriteria { EventDate = eventDate });

        return new AssignmentSnapshot
        {
            Matches = (await matchesTask).ToDictionary(m => m.MatchId),
            Assignments = (await assignmentsTask).GroupBy(a => a.MatchId).ToDictionary(a => a.Key, a => a.ToList()),
            MatchSettings = (await matchSettingsTask).GroupBy(a => a.MatchId).ToDictionary(a => a.Key, a => a.ToList())
        };
    }
}