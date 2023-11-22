using OWP.Core.Extensions;
using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Helpers;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedAssignmentNoteGenerator
{
    List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees);
}

public class ChangedAssignmentNoteGeneratorImp : ChangedAssignmentNoteGenerator
{
    internal class MatchAssignmentComparer : IEqualityComparer<MatchAssignment>
    {
        public bool Equals(MatchAssignment x, MatchAssignment y)
        {
            var isSameEvent = x.OSMatchId == y.OSMatchId && x.BetTypeName == y.BetTypeName;
            var isSameAssignment = x.AssignmentType == y.AssignmentType && x.UserId == y.UserId && x.AssignmentNote != y.AssignmentNote;

            return isSameEvent && isSameAssignment;
        }

        public int GetHashCode(MatchAssignment obj)
        {
            return obj.OSMatchId.GetHashCode();
        }
    }

    public List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees)
        => GetChangedAssignmentNotes(snapshot).ToList();

    public static IEnumerable<ChangedValue> GetChangedAssignmentNotes(ChangedMatchSnapshot snapshot)
    {
        var oldAssignments = snapshot.OldSnapshot.MatchAssignments.GetValue(snapshot.OldMatch.OSMatchId, new List<MatchAssignment>());
        var newAssignments = snapshot.NewSnapshot.MatchAssignments.GetValue(snapshot.NewMatch.OSMatchId, new List<MatchAssignment>());
        var oldMatches = snapshot.OldSnapshot.Matches;
        var changedAssignments = GetChangedAssignments(oldAssignments, newAssignments);

        foreach (var assignment in changedAssignments)
        {
            var match = oldMatches.GetValue(assignment.OSMatchId);
            var oldValue = UpdatedValueHelper.GetValue(assignment.Username, oldAssignments.FirstOrDefault(x => IsSameAssignment(x, assignment))?.AssignmentNote ?? "");
            var newValue = UpdatedValueHelper.GetValue(assignment.Username, newAssignments.FirstOrDefault(x => IsSameAssignment(x, assignment))?.AssignmentNote ?? "");

            yield return new ChangedValue
            {
                MessageTypeId = (int)MsgTemplateCode.EditAssignmentNote,
                OldValue = oldValue,
                NewValue = newValue,
                AssignmentNote = assignment.AssignmentNote,
                AssignmentType = assignment.AssignmentType,
                BetTypeName = assignment.BetTypeName,
                OSMatchId = match.OSMatchId,
                MatchNo = $"{match.MatchNo}",
                AdminsiteMatchID = match.AdminsiteMatchId,
                League = match.League,
                EventName = UpdatedValueHelper.CreateEventName(match),
                UserId = assignment.UserId,
                Username = assignment.Username
            };
        }
    }

    private static IEnumerable<MatchAssignment> GetChangedAssignments(IEnumerable<MatchAssignment> oldAssignments, IEnumerable<MatchAssignment> newAssignments)
        => newAssignments.Intersect(oldAssignments, new MatchAssignmentComparer()).ToList();

    private static bool IsSameAssignment(MatchAssignment x, MatchAssignment y)
    {
        var isSameEvent = x.OSMatchId == y.OSMatchId && x.BetTypeName == y.BetTypeName;
        var isSameAssignment = x.AssignmentType == y.AssignmentType && x.UserId == y.UserId;

        return isSameEvent && isSameAssignment;
    }
}
