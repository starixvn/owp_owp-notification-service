using OWP.Core.Extensions;
using OWP.PreMatchNotification.LinkedMatch.Models;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.LinkedMatch.Services;

public interface ChangedLiveMatchGenerator
{
    List<ChangedValue> Create(ChangedSnapshot changedSnapshot, Employee employees);
}

public class ChangedLiveMatchGeneratorImp : ChangedLiveMatchGenerator
{
    public List<ChangedValue> Create(ChangedSnapshot changedSnapshot, Employee employees)
    {
        var cancelLiveMatchesChangedValue = CreateDeletedMatchesChangedValue(changedSnapshot, employees);
        var addLiveMatchesChangedValue = CreateInsertedMatchesChangedValue(changedSnapshot, employees);

        return cancelLiveMatchesChangedValue.Union(addLiveMatchesChangedValue).ToList();
    }

    public IEnumerable<ChangedValue> CreateDeletedMatchesChangedValue(ChangedSnapshot changedSnapshot, Employee employees)
        => CreateMatchesChangedValue(true, changedSnapshot, employees);

    public IEnumerable<ChangedValue> CreateInsertedMatchesChangedValue(ChangedSnapshot changedSnapshot, Employee employees)
        => CreateMatchesChangedValue(false, changedSnapshot, employees);

    public IEnumerable<ChangedValue> CreateMatchesChangedValue(bool isDeletedMatches, ChangedSnapshot changedSnapshot, Employee employees)
    {
        var matches = isDeletedMatches ? changedSnapshot.DeletedMatches : changedSnapshot.InsertedMatches;
        var messageTypeId = isDeletedMatches
            ? (int)MsgTemplateCode.LGACancelLiveMatch
            : (int)MsgTemplateCode.LGAAddLiveMatch;
        var lstEmployees = isDeletedMatches ? [] : employees.AllLSTEmployees;

        foreach (var match in matches)
        {
            var prematchInfosite = employees.PrematchInfosites.GetValue(match.InfoSiteMatchId);
            var prematchAssigneeIds = prematchInfosite.SelectMany(x => x.PrematchAssigneeIds).ToList();
            var infositeInfo = prematchInfosite[0];

            yield return new ChangedValue
            {
                MessageTypeId = messageTypeId,
                League = infositeInfo.LeagueName,
                Home = infositeInfo.HomeName,
                Away = infositeInfo.AwayName,
                KoTime = infositeInfo.EventDateTime.ToString("hh:mm tt"),
                ReceivedUserIds = !prematchAssigneeIds.Any()
                    ? lstEmployees.Union(employees.AllPrematchEmployees).Distinct().ToList()
                    : lstEmployees.Union(prematchAssigneeIds).Distinct().ToList()
            };
        }
    }
}