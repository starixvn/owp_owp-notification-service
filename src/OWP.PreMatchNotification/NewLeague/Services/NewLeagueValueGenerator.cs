
using OWP.PreMatchNotification.NewLeague.Models;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.NewLeague.Services;

public interface NewLeagueValueGenerator
{
    IEnumerable<ChangedValue> Create(
        IList<PreMatchLeague> oldSnapshot,
        IList<PreMatchLeague> newSnapshot, IList<Assignee> assignees,
        IList<InfoSiteMatch> infoSiteMatches);
}

public class NewLeagueValueGeneratorImp : NewLeagueValueGenerator
{
    private readonly DateTimeHelper dateTimeHelper;

    public NewLeagueValueGeneratorImp(DateTimeHelper dateTimeHelper)
    {
        this.dateTimeHelper = dateTimeHelper;
    }

    public IEnumerable<ChangedValue> Create(IList<PreMatchLeague> oldSnapshot,
        IList<PreMatchLeague> newSnapshot,
        IList<Assignee> assignees,
        IList<InfoSiteMatch> infoSiteMatches)
    {
        var newLeagues = GetNewPreMatchLeagues(oldSnapshot, newSnapshot, infoSiteMatches);
        var changedValue = newLeagues.Select(league =>
            new ChangedValue
            {
                League = league.OWPLeagueName,
                ReceivedUserIds = assignees.Select(x => x.AssigneeId).ToList(),
                MessageTypeId = (int)MsgTemplateCode.NewLeague
            }).ToList();

        return changedValue.ToList();
    }

    private IEnumerable<PreMatchLeague> GetNewPreMatchLeagues(IList<PreMatchLeague> oldSnapshot,
        IList<PreMatchLeague> newSnapshot, IList<InfoSiteMatch> infoSiteMatches)
    {
        var newLeagues = newSnapshot.Except(oldSnapshot, new PreMatchLeague()).ToList();
        var currentDate = dateTimeHelper.GetCurrentDate();
        var fromDate = currentDate.AddHours(12);
        var toDate = currentDate.AddDays(1).AddHours(11).AddMinutes(59);

        var preMatchLeagues = new List<PreMatchLeague>();
        foreach (var league in newLeagues)
        {
            var matches = infoSiteMatches.Where(x =>
                x.OWPLeagueId == league.OWPLeagueId &&
                x.EventDateTime >= fromDate &&
                x.EventDateTime <= toDate);

            if (matches.Any())
            {
                preMatchLeagues.Add(league);
            }
        }
        return preMatchLeagues;
    }
}