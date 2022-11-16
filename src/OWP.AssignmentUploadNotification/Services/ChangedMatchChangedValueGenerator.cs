namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using OWP.AssignmentUploadNotification.Constants;
    using OWP.Core.Extensions;

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

        public ChangedMatchChangedValueGeneratorImp(
            ChangedMatchInfoGenerator changedMatchInfoGenerator,
            ChangedAssignmentGenerator changedAssignmentGenerator,
            ChangedMatchSettingGenerator changedMatchSettingGenerator,
            ChangedMatchStatusGenerator changedMatchStatusGenerator)
        {
            this.changedMatchStatusGenerator = changedMatchStatusGenerator;
            this.changedMatchInfoGenerator = changedMatchInfoGenerator;
            this.changedAssignmentGenerator = changedAssignmentGenerator;
            this.changedMatchSettingGenerator = changedMatchSettingGenerator;
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
            var assigneeOfMatch = GetAssignmentByMatchId(matchId, snapshot);
            var assigneeOfMainMatch = GetAssignmentOfMainMatch(matchId, snapshot);
            var assigneeOfMinorMatch = GetAssignmentOfMinorMatch(matchId, snapshot);

            var allAssignment = assigneeOfMatch.Union(assigneeOfMainMatch)
                                .Union(assigneeOfMinorMatch)
                                .ToList();

            return allAssignment.Select(a => new Assignee
            {
                AssigneeId = a.UserId,
                AssigneeName = a.UserName
            })
            .Distinct(new Assignee())
            .ToList();
        }

        private static List<MatchAssignment> GetAssignmentOfMainMatch(int matchId, AssignmentSnapshot snapshot)
        {
            var match = snapshot.Matches[matchId];
            var parentMatchId = match.ParentMatchId;
            var assignmentOfMainMatch = GetAssignmentByMatchId(parentMatchId, snapshot);
            var assignmentOfSpecificMatch = GetAllAssignmentOfSpecificMatch(matchId, snapshot, parentMatchId);

            return assignmentOfMainMatch.Union(assignmentOfSpecificMatch).ToList();
        }

        private static List<MatchAssignment> GetAllAssignmentOfSpecificMatch(int matchId, AssignmentSnapshot snapshot, int parentMatchId)
        {
            var specificMap = new Dictionary<int, int>
            {
                { (int)MinorBettype.NoOfCorner, (int)MinorBettype.Specific15MinsCorners },
                { (int)MinorBettype.Specific15MinsCorners, (int)MinorBettype.NoOfCorner },
                { (int)MinorBettype.TotalBooking, (int)MinorBettype.Specific15MinsTotalBookings },
                { (int)MinorBettype.Specific15MinsTotalBookings, (int)MinorBettype.TotalBooking }
            };

            var bettypeId = snapshot.Matches[matchId].MatchType;
            if (specificMap.ContainsKey(bettypeId))
            {
                var allMatches = snapshot.Matches.Values;
                var minorMatchId = allMatches
                                .Where(x => x.ParentMatchId == parentMatchId && x.MatchType == specificMap[bettypeId])
                                .Select(x => x.MatchId)
                                .FirstOrDefault();
                return GetAssignmentByMatchId(minorMatchId, snapshot);
            }

            return new List<MatchAssignment>();
        }

        private static List<MatchAssignment> GetAssignmentOfMinorMatch(int matchId, AssignmentSnapshot snapshot)
        {
            var assignments = new List<MatchAssignment>();
            var allMatches = snapshot.Matches.Values;
            var minorMatchIds = allMatches
                            .Where(x => x.ParentMatchId == matchId)
                            .Select(a => a.MatchId)
                            .ToList();
            foreach (var minorMatchId in minorMatchIds)
            {
                assignments.AddRange(GetAssignmentByMatchId(minorMatchId, snapshot));
            }

            return assignments;
        }

        private static List<MatchAssignment> GetAssignmentByMatchId(int matchId, AssignmentSnapshot snapshot) => snapshot.Assignments.GetValue(
                matchId,
                new List<MatchAssignment>());
    }
}
