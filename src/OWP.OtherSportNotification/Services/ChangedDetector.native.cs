using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedDetector
{
    List<MatchAssignment> GetDeletedAssignments(List<MatchAssignment> oldAssignments, List<MatchAssignment> newAssignments);

    List<MatchAssignment> GetInsertedAssignments(List<MatchAssignment> oldAssignments, List<MatchAssignment> newAssignments);

    ChangedSnapshot GetChangedSnapshot(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot);
}

public class ChangedDetectorImp : ChangedDetector
{
    internal class MatchAssignmentComparer : IEqualityComparer<MatchAssignment>
    {
        public bool Equals(MatchAssignment x, MatchAssignment y)
        {
            var sameEvent = x.OSMatchId == y.OSMatchId && x.BetTypeName == y.BetTypeName;
            var sameAssignment = x.AssignmentType == y.AssignmentType && x.UserId == y.UserId;

            return sameEvent && sameAssignment;
        }

        public int GetHashCode(MatchAssignment obj)
        {
            return obj.OSMatchId.GetHashCode();
        }
    }

    public ChangedSnapshot GetChangedSnapshot(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        var deletedMatches = GetDeletedMatches(oldSnapshot, newSnapshot);
        var insertedMatches = GetInsertedMatches(oldSnapshot, newSnapshot);
        var changedMatches = GetChangedMatches(oldSnapshot, newSnapshot);

        return new ChangedSnapshot
        {
            OldSnapshot = oldSnapshot,
            NewSnapshot = newSnapshot,
            InsertedMatches = insertedMatches,
            DeletedMatches = deletedMatches,
            ChangedMatches = changedMatches
        };
    }

    public List<MatchAssignment> GetDeletedAssignments(List<MatchAssignment> oldAssignments, List<MatchAssignment> newAssignments)
    {
        return oldAssignments.Except(newAssignments, new MatchAssignmentComparer()).ToList();
    }

    public List<MatchAssignment> GetInsertedAssignments(List<MatchAssignment> oldAssignments, List<MatchAssignment> newAssignments)
    {
        return newAssignments.Except(oldAssignments, new MatchAssignmentComparer()).ToList();
    }

    public List<int> GetDeletedMatches(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        return oldSnapshot.Matches.Keys.Except(newSnapshot.Matches.Keys).ToList();
    }

    public List<int> GetInsertedMatches(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        return newSnapshot.Matches.Keys.Except(oldSnapshot.Matches.Keys).ToList();
    }

    public List<int> GetChangedMatches(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        return newSnapshot.Matches.Keys.Intersect(oldSnapshot.Matches.Keys).ToList();
    }
}
