namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Constants;
    using Extensions;
    using Models;
    using OWP.Core.Extensions;

    public interface InsertedMatchChangedValueGenerator
    {
        IEnumerable<ChangedValue> Create(List<int> matcheIds, AssignmentSnapshot snapshot);
    }

    public class InsertedMatchChangedValueGeneratorImp : InsertedMatchChangedValueGenerator
    {
        private const string EVENT_VS = " -vs- ";

        public IEnumerable<ChangedValue> Create(List<int> matcheIds, AssignmentSnapshot snapshot)
        {
            return CreateEnumerable(matcheIds, snapshot).SelectMany(v => v).Distinct().ToList();
        }

        public IEnumerable<List<ChangedValue>> CreateEnumerable(List<int> matcheIds, AssignmentSnapshot snapshot)
        {
            foreach (var matchId in matcheIds)
            {
                var assignments = snapshot.Assignments.GetValue(matchId, new List<MatchAssignment>())
                                          .Where(x => x.UserId != 0)
                                          .ToList();
                var assignmentChangedValues = GetInsertedAssignmentChangedValues(snapshot.Matches, assignments).ToList();
                yield return assignmentChangedValues.Distinct(new AssignmentChangedValueComparer()).ToList();
            }
        }

        public IEnumerable<ChangedValue> GetInsertedAssignmentChangedValues(Dictionary<int, GeneralMatch> allMatches, List<MatchAssignment> assignments)
        {
            foreach (var assignment in assignments)
            {
                var match = allMatches[assignment.MatchId];
                var mainMatch = allMatches.GetValue(match.ParentMatchId, match);
                var bettypeName = BettypeNameExtensions.GetBettypeNameForAssignment(assignment.BetType, match.MatchType, match.MinorBettypeName);

                var messageTypeId = assignment.AssignmentType == 1
                    ? (int)MsgTemplateCode.AssignmentInsert
                    : (int)MsgTemplateCode.OtherAssignmentInsert;
                var newValue = UpdatedValueHelper.GetValue(assignment.AssignmentType, assignment.UserName, assignment.AssignmentNote);
                yield return new ChangedValue
                {
                    MessageTypeId = messageTypeId,
                    BetTypeId = assignment.BetType,
                    OldValue = null,
                    NewValue = newValue,
                    BetTypeName = bettypeName,
                    MatchId = match.MatchId,
                    MatchNo = mainMatch.MatchNo,
                    League = mainMatch.LeagueName,
                    EventName = mainMatch.Home + EVENT_VS + mainMatch.Away,
                    AssigneeId = assignment.UserId,
                    AssigneeName = assignment.UserName,
                    AssignmentType = assignment.AssignmentType,
                    AssignmentNote = assignment.AssignmentNote
                };
            }
        }
    }
}
