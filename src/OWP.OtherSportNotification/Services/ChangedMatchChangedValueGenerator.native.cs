using OWP.Core.Extensions;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedMatchChangedValueGenerator
{
    List<ChangedValue> Create(List<int> matchIds, AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot);
}

public class ChangedMatchChangedValueGeneratorImp(
    ChangedMatchInfoGenerator changedMatchInfoGenerator,
    ChangedAssignmentGenerator changedAssignmentGenerator,
    ChangedMatchSettingGenerator changedMatchSettingGenerator,
    ChangedMatchStatusGenerator changedMatchStatusGenerator,
    ChangedAssignmentNoteGenerator changedAssignmentNoteGenerator)
    : ChangedMatchChangedValueGenerator
{
    public List<ChangedValue> Create(List<int> matchIds, AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        var changedValues = CreateEnumerableChangeValues(matchIds, oldSnapshot, newSnapshot);

        return changedValues.SelectMany(v => v).ToList();
    }

    public IEnumerable<List<ChangedValue>> CreateEnumerableChangeValues(List<int> matchIds, AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        foreach (var matchId in matchIds)
        {
            var changedMatchSnapshot = new ChangedMatchSnapshot
            {
                OldMatch = oldSnapshot.Matches[matchId],
                NewMatch = newSnapshot.Matches[matchId],
                OldSnapshot = oldSnapshot,
                NewSnapshot = newSnapshot
            };

            var existingAssignees = GetAllIndirectAssignees(matchId, oldSnapshot, newSnapshot);

            yield return changedMatchStatusGenerator.Create(changedMatchSnapshot, existingAssignees);
            yield return changedMatchInfoGenerator.Create(oldSnapshot.Matches[matchId], newSnapshot.Matches[matchId], existingAssignees);
            yield return changedMatchSettingGenerator.Create(changedMatchSnapshot, matchId, existingAssignees);
            yield return changedAssignmentGenerator.Create(changedMatchSnapshot, existingAssignees);
            yield return changedAssignmentNoteGenerator.Create(changedMatchSnapshot, existingAssignees);
        }
    }

    public static List<Assignee> GetAllIndirectAssignees(int matchId, AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        var newAssignees = GetAllAssigneesByMatchId(matchId, newSnapshot);
        var oldAssignees = GetAllAssigneesByMatchId(matchId, oldSnapshot);

        return newAssignees.Intersect(oldAssignees, new Assignee())
                           .Distinct(new Assignee())
                           .Where(x => x.AssigneeId != 0)
                           .ToList();
    }

    private static IEnumerable<Assignee> GetAllAssigneesByMatchId(int matchId, AssignmentSnapshot snapshot)
    {
        var assigneeOfMatch = snapshot.MatchAssignments.GetValue(matchId, []);

        return assigneeOfMatch.Select(a => new Assignee
        {
            AssigneeId = a.UserId,
            AssigneeName = a.Username
        })
        .Distinct(new Assignee())
        .ToList();
    }
}
