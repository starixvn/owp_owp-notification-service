using OWP.Core.Extensions;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedMatchChangedValueGenerator
{
    List<ChangedValue> Create(List<int> matcheIds, AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot);
}

public class ChangedMatchChangedValueGeneratorImp : ChangedMatchChangedValueGenerator
{
    private readonly ChangedMatchStatusGenerator changedMatchStatusGenerator;
    private readonly ChangedMatchInfoGenerator changedMatchInfoGenerator;
    private readonly ChangedAssignmentGenerator changedAssignmentGenerator;
    private readonly ChangedMatchSettingGenerator changedMatchSettingGenerator;
    private readonly ChangedAssignmentNoteGenerator changedAssignmentNoteGenerator;

    public ChangedMatchChangedValueGeneratorImp(
        ChangedMatchInfoGenerator changedMatchInfoGenerator,
        ChangedAssignmentGenerator changedAssignmentGenerator,
        ChangedMatchSettingGenerator changedMatchSettingGenerator,
        ChangedMatchStatusGenerator changedMatchStatusGenerator,
        ChangedAssignmentNoteGenerator changedAssignmentNoteGenerator)
    {
        this.changedMatchStatusGenerator = changedMatchStatusGenerator;
        this.changedMatchInfoGenerator = changedMatchInfoGenerator;
        this.changedAssignmentGenerator = changedAssignmentGenerator;
        this.changedMatchSettingGenerator = changedMatchSettingGenerator;
        this.changedAssignmentNoteGenerator = changedAssignmentNoteGenerator;
    }

    public List<ChangedValue> Create(List<int> matcheIds, AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        var changedValues = CreateEnumerableChangeValues(matcheIds, oldSnapshot, newSnapshot);

        return changedValues.SelectMany(v => v).ToList();
    }

    public IEnumerable<List<ChangedValue>> CreateEnumerableChangeValues(List<int> matcheIds, AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        foreach (var matchId in matcheIds)
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

    private static List<Assignee> GetAllAssigneesByMatchId(int matchId, AssignmentSnapshot snapshot)
    {
        var assigneeOfMatch = snapshot.MatchAssignments.GetValue(matchId, new List<MatchAssignment>());

        return assigneeOfMatch.Select(a => new Assignee
        {
            AssigneeId = a.UserId,
            AssigneeName = a.Username
        })
        .Distinct(new Assignee())
        .ToList();
    }
}
