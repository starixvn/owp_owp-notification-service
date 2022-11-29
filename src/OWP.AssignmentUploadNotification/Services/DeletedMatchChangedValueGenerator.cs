namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Constants;
    using Extensions;
    using Models;
    using OWP.Core.Extensions;

    public interface DeletedMatchChangedValueGenerator
    {
        IEnumerable<ChangedValue> Create(List<int> matcheIds, AssignmentSnapshot snapshot);
    }

    public class DeletedMatchChangedValueGeneratorImp : DeletedMatchChangedValueGenerator
    {
        private const string EVENT_VS = " -vs- ";

        public IEnumerable<ChangedValue> Create(List<int> matchIds, AssignmentSnapshot snapshot)
        {
            foreach (var matchId in matchIds)
            {
                var match = snapshot.Matches.GetValue(matchId);
                var mainMatch = snapshot.Matches.GetValue(match.ParentMatchId, match);
                var assignments = snapshot.Assignments.GetValue(matchId, new List<MatchAssignment>())
                                          .Where(x => x.UserId != 0)
                                          .ToList();
                foreach (var assignment in assignments)
                {
                    yield return GetDeletedAssignmentOfMatchChanged(assignment, match, mainMatch);
                }

            }
        }

        public ChangedValue GetDeletedAssignmentOfMatchChanged(MatchAssignment assignment, GeneralMatch match, GeneralMatch mainMatch)
        {
            var bettypeName = BettypeNameExtensions.GetBettypeNameForAssignment(assignment.BetType, match.MatchType, match.MinorBettypeName);
            var messageTypeId = assignment.AssignmentType == 1
                ? (int)MsgTemplateCode.AssignmentDelete
                : (int)MsgTemplateCode.OtherAssignmentDelete;
            var oldValue = UpdatedValueHelper.GetValue(assignment.AssignmentType, assignment.UserName, assignment.AssignmentNote);
            return new ChangedValue
            {
                MessageTypeId = messageTypeId,
                BetTypeId = assignment.BetType,
                OldValue = oldValue,
                NewValue = null,
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
