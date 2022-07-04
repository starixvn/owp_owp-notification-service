using OWP.Core.Extensions;
using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Helpers;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedAssignmentGenerator
{
    List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees);
}

public class ChangedAssignmentGeneratorImp : ChangedAssignmentGenerator
{
    private readonly ChangedDetector changeDetector;

    public ChangedAssignmentGeneratorImp(ChangedDetector changeDetector)
    {
        this.changeDetector = changeDetector;
    }

    public List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees)
    {
        var deletedAssignmentChangedValues = GetDeletedAssignmentChangedValues(snapshot).ToList();
        var insertedAssignmentChangedValues = GetInsertedAssignmentChangedValues(snapshot).ToList();

        var allChangedValues = deletedAssignmentChangedValues.Union(insertedAssignmentChangedValues).ToList();
        var indirectChangedValues = GetIndirectChangedValues(allChangedValues, existingAssignees).ToList();

        return allChangedValues.Where(x => x.UserId != 0)
            .Union(indirectChangedValues)
            .Distinct(new AssignmentChangedValueComparer())
            .ToList();
    }

    public IEnumerable<ChangedValue> GetDeletedAssignmentChangedValues(ChangedMatchSnapshot snapshot)
    {
        var oldAssignments = snapshot.OldSnapshot.MatchAssignments.GetValue(snapshot.OldMatch.OSMatchId, new List<MatchAssignment>());
        var newAssignments = snapshot.NewSnapshot.MatchAssignments.GetValue(snapshot.NewMatch.OSMatchId, new List<MatchAssignment>());
        var oldMatches = snapshot.OldSnapshot.Matches;

        var deletedAssignments = changeDetector.GetDeletedAssignments(oldAssignments, newAssignments);

        foreach (var assignment in deletedAssignments)
        {
            var match = oldMatches.GetValue(assignment.OSMatchId);

            var messageTypeId = assignment.AssignmentType == 1
                ? (int)MsgTemplateCode.AssignmentDelete
                : (int)MsgTemplateCode.OtherAssignmentDelete;
            var newAssignment = newAssignments.FirstOrDefault(x => IsSameAssignment(x, assignment));
            var oldValue = UpdatedValueHelper.GetValue(assignment.Username, assignment.AssignmentNote);
            var newValue = newAssignment != null ? UpdatedValueHelper.GetValue(newAssignment.Username, newAssignment.AssignmentNote) : string.Empty;

            yield return new ChangedValue
            {
                MessageTypeId = messageTypeId,
                OldValue = oldValue,
                AssignmentNote = assignment.AssignmentNote,
                AssignmentType = assignment.AssignmentType,
                NewValue = newValue,
                BetTypeName = assignment.BetTypeName,
                OSMatchId = match.OSMatchId,
                MatchNo = $"{match.MatchNo}",
                League = match.League,
                EventName = UpdatedValueHelper.CreateEventName(match),
                UserId = assignment.UserId,
                Username = assignment.Username,
                EventDateTime = match.DateTime
            };
        }
    }

    public IEnumerable<ChangedValue> GetInsertedAssignmentChangedValues(ChangedMatchSnapshot snapshot)
    {
        var oldAssignments = snapshot.OldSnapshot.MatchAssignments.GetValue(snapshot.OldMatch.OSMatchId, new List<MatchAssignment>());
        var newAssignments = snapshot.NewSnapshot.MatchAssignments.GetValue(snapshot.NewMatch.OSMatchId, new List<MatchAssignment>());
        var oldMatches = snapshot.OldSnapshot.Matches;

        var insertedAssignments = changeDetector.GetInsertedAssignments(oldAssignments, newAssignments);

        foreach (var assignment in insertedAssignments)
        {
            var match = oldMatches.GetValue(assignment.OSMatchId);

            var messageTypeId = assignment.AssignmentType == 1
                ? (int)MsgTemplateCode.AssignmentInsert
                : (int)MsgTemplateCode.OtherAssignmentInsert;
            var newValue = UpdatedValueHelper.GetValue(assignment.Username, assignment.AssignmentNote);
            var oldAssignment = oldAssignments.FirstOrDefault(x => IsSameAssignment(x, assignment));
            var oldValue = oldAssignment != null ? UpdatedValueHelper.GetValue(oldAssignment.Username, oldAssignment.AssignmentNote) : string.Empty;

            yield return new ChangedValue
            {
                MessageTypeId = messageTypeId,
                OldValue = oldValue,
                NewValue = newValue,
                AssignmentNote = assignment.AssignmentNote,
                AssignmentType = assignment.AssignmentType,
                BetTypeName = assignment.BetTypeName,
                OSMatchId = match.OSMatchId,
                MatchNo = $"{match.MatchNo}",
                League = match.League,
                EventName = UpdatedValueHelper.CreateEventName(match),
                UserId = assignment.UserId,
                Username = assignment.Username,
                EventDateTime = match.DateTime
            };
        }
    }

    public static IEnumerable<ChangedValue> GetIndirectChangedValues(
        List<ChangedValue> allChangedValues,
        List<Assignee> existingAssignees)
    {
        return existingAssignees
            .Select(t => allChangedValues.Where(a => a.UserId != t.AssigneeId).Select(a => GetIndirectChangedValue(a, t)))
            .SelectMany(a => a);
    }

    public static ChangedValue GetIndirectChangedValue(ChangedValue changedValue, Assignee assignee)
    {
        return new ChangedValue
        {
            MessageTypeId = changedValue.MessageTypeId,
            OldValue = changedValue.OldValue,
            NewValue = changedValue.NewValue,
            BetTypeName = changedValue.BetTypeName,
            OSMatchId = changedValue.OSMatchId,
            MatchNo = changedValue.MatchNo,
            League = changedValue.League,
            EventName = changedValue.EventName,
            UserId = assignee.AssigneeId,
            Username = assignee.AssigneeName,
            AssignmentType = changedValue.AssignmentType,
            AssignmentNote = changedValue.AssignmentNote,
            IsIndirectChangedAssignment = true,
            EventDateTime = changedValue.EventDateTime
        };
    }

    private static bool IsSameAssignment(MatchAssignment x, MatchAssignment y)
        => x.OSMatchId == y.OSMatchId && x.BetTypeName == y.BetTypeName && x.AssignmentType == y.AssignmentType;
}
