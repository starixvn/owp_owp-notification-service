using OWP.Core.Extensions;
using OWP.PreMatchNotification.LinkedMatch.Models;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.LinkedMatch.Services;

public interface ChangedMatchStatusGenerator
{
    List<ChangedValue> Create(ChangedSnapshot changedSnapshot, Employee employees);
}

public class ChangedMatchStatusGeneratorImp : ChangedMatchStatusGenerator
{
    public List<ChangedValue> Create(ChangedSnapshot changedSnapshot, Employee employees)
    {
        return changedSnapshot.ChangedMatches.Select(x => CreateMatchStatusChangedValue(x, changedSnapshot, employees)).Where(s => s != null).ToList();
    }

    public ChangedValue CreateMatchStatusChangedValue(GeneralMatch changedMatch, ChangedSnapshot changedSnapshot, Employee employees)
    {
        var oldMatch = changedSnapshot.OldSnapshot.FirstOrDefault(x => x.MatchId == changedMatch.MatchId);
        var newMatch = changedSnapshot.NewSnapshot.FirstOrDefault(x => x.MatchId == changedMatch.MatchId);

        if (oldMatch!.MatchStatus != newMatch!.MatchStatus)
        {
            var prematchInfosite = employees.PrematchInfosites.GetValue(newMatch.InfoSiteMatchId);
            var prematchAssigneeIds = prematchInfosite.SelectMany(x => x.PrematchAssigneeIds);
            var infositeInfo = prematchInfosite[0];

            return new ChangedValue
            {
                MessageTypeId = (int)MsgTemplateCode.LGAChangeMatchStatus,
                League = infositeInfo.LeagueName,
                Home = infositeInfo.HomeName,
                Away = infositeInfo.AwayName,
                OldValue = oldMatch.MatchStatus,
                NewValue = newMatch.MatchStatus,
                ReceivedUserIds = employees.AllLSTEmployees.Union(prematchAssigneeIds).Distinct().ToList()
            };
        }

        return null;
    }
}