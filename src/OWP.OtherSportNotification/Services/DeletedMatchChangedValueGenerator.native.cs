using OWP.Core.Extensions;
using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Helpers;
using OWP.OtherSportNotification.Models;
using System.Globalization;

namespace OWP.OtherSportNotification.Services;

public interface DeletedMatchChangedValueGenerator
{
    IEnumerable<ChangedValue> Create(List<int> matchIds, AssignmentSnapshot snapshot);
}

public class DeletedMatchChangedValueGeneratorImp : DeletedMatchChangedValueGenerator
{
    public IEnumerable<ChangedValue> Create(List<int> matchIds, AssignmentSnapshot snapshot)
    {
        foreach (var matchId in matchIds)
        {
            var match = snapshot.Matches.GetValue(matchId);
            var assignments = snapshot.MatchAssignments.GetValue(matchId, [])
                                      .Where(x => x.UserId != 0)
                                      .ToList();
            foreach (var assignment in assignments)
            {
                yield return GetDeletedAssignmentOfMatchChanged(assignment, match);
            }
        }
    }

    public ChangedValue GetDeletedAssignmentOfMatchChanged(MatchAssignment assignment, MatchInfo match)
    {
        var messageTypeId = assignment.AssignmentType == 1
            ? (int)MsgTemplateCode.AssignmentDelete
            : (int)MsgTemplateCode.OtherAssignmentDelete;
        var oldValue = UpdatedValueHelper.GetValue(assignment.Username, assignment.AssignmentNote);

        return new ChangedValue
        {
            MessageTypeId = messageTypeId,
            OldValue = oldValue,
            NewValue = null,
            BetTypeName = assignment.BetTypeName,
            OSMatchId = match.OSMatchId,
            MatchNo = match.MatchNo.ToString(CultureInfo.CurrentCulture),
            AdminsiteMatchID = match.AdminsiteMatchId,
            League = match.League,
            EventName = UpdatedValueHelper.CreateEventName(match),
            UserId = assignment.UserId,
            Username = assignment.Username,
            AssignmentType = assignment.AssignmentType,
            AssignmentNote = assignment.AssignmentNote,
            EventDateTime = match.DateTime
        };
    }
}
