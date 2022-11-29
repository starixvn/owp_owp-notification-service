namespace OWP.AssignmentUploadNotification.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Constants;
    using Extensions;
    using Fanex.Logging;
    using Models;
    using Newtonsoft.Json;
    using OWP.Core.Extensions;

    public interface ChangedAssignmentGenerator
    {
        List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees);
    }

    public class ChangedAssignmentGeneratorImp : ChangedAssignmentGenerator
    {
        private const string EVENT_VS = " -vs- ";

        private readonly ILogger _logger;

        private readonly ChangedDetector changeDetector;

        public ChangedAssignmentGeneratorImp(ChangedDetector changeDetector, ILogger logger)
        {
            this.changeDetector = changeDetector;
            _logger = logger;
        }

        public List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees)
        {
            var deletedAssignmentChangedValues = GetDeletedAssignmentChangedValues(snapshot).ToList();
            var insertedAssignmentChangedValues = GetInsertedAssignmentChangedValues(snapshot).ToList();

            var allChangedValues = deletedAssignmentChangedValues.Union(insertedAssignmentChangedValues).ToList();
            var indirectChangedValues = GetIndirectChangedValues(snapshot, allChangedValues, existingAssignees).ToList();

            return allChangedValues.Where(x => x.AssigneeId != 0)
                .Union(indirectChangedValues)
                .Distinct(new AssignmentChangedValueComparer())
                .ToList();
        }

        public IEnumerable<ChangedValue> GetDeletedAssignmentChangedValues(ChangedMatchSnapshot snapshot)
        {
            var oldAssignments = snapshot.OldSnapshot.Assignments.GetValue(snapshot.OldMatch.MatchId, new List<MatchAssignment>());
            var newAssignments = snapshot.NewSnapshot.Assignments.GetValue(snapshot.NewMatch.MatchId, new List<MatchAssignment>());
            var oldMatches = snapshot.OldSnapshot.Matches;

            var deletedAssignments = changeDetector.GetDeletedAssignments(oldAssignments, newAssignments);

            foreach (var assignment in deletedAssignments)
            {
                var match = oldMatches.GetValue(assignment.MatchId);
                var mainMatch = oldMatches.GetValue(match.ParentMatchId, match);
                var bettypeName = BettypeNameExtensions.GetBettypeNameForAssignment(assignment.BetType, match.MatchType, match.MinorBettypeName);

                var messageTypeId = assignment.AssignmentType == 1
                    ? (int)MsgTemplateCode.AssignmentDelete
                    : (int)MsgTemplateCode.OtherAssignmentDelete;
                var oldValue = UpdatedValueHelper.GetValue(assignment.AssignmentType, assignment.UserName, assignment.AssignmentNote);
                yield return new ChangedValue
                {
                    MessageTypeId = messageTypeId,
                    BetTypeId = assignment.BetType,
                    OldValue = oldValue,
                    AssignmentNote = assignment.AssignmentNote,
                    AssignmentType = assignment.AssignmentType,
                    NewValue = null,
                    BetTypeName = bettypeName,
                    MatchId = match.MatchId,
                    MatchNo = mainMatch.MatchNo,
                    League = mainMatch.LeagueName,
                    EventName = mainMatch.Home + EVENT_VS + mainMatch.Away,
                    AssigneeId = assignment.UserId,
                    AssigneeName = assignment.UserName
                };
            }
        }

        public IEnumerable<ChangedValue> GetInsertedAssignmentChangedValues(ChangedMatchSnapshot snapshot)
        {
            var oldAssignments = GetAssignmentsOfMatchWhichIsChangedToLive(snapshot);
            var newAssignments = snapshot.NewSnapshot.Assignments.GetValue(snapshot.NewMatch.MatchId, new List<MatchAssignment>());
            var oldMatches = snapshot.OldSnapshot.Matches;

            var insertedAssignments = changeDetector.GetInsertedAssignments(oldAssignments, newAssignments);
            if (insertedAssignments?.Count > 0)
            {
                _logger.Info("insertedAssignments" + JsonConvert.SerializeObject(insertedAssignments));
            }
            foreach (var assignment in insertedAssignments)
            {
                var match = oldMatches.GetValue(assignment.MatchId);
                var mainMatch = oldMatches.GetValue(match.ParentMatchId, match);
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
                    AssignmentNote = assignment.AssignmentNote,
                    AssignmentType = assignment.AssignmentType,
                    BetTypeName = bettypeName,
                    MatchId = match.MatchId,
                    MatchNo = mainMatch.MatchNo,
                    League = mainMatch.LeagueName,
                    EventName = mainMatch.Home + EVENT_VS + mainMatch.Away,
                    AssigneeId = assignment.UserId,
                    AssigneeName = assignment.UserName,
                    IsChangedToLiveMatchStatus = IsChangedToLiveMatchStatus(snapshot)
                };
            }
        }

        public static List<MatchAssignment> GetAssignmentsOfMatchWhichIsChangedToLive(ChangedMatchSnapshot snapshot)
        {
            return IsChangedToLiveMatchStatus(snapshot)
                ? new List<MatchAssignment>()
                : snapshot.OldSnapshot.Assignments.GetValue(snapshot.OldMatch.MatchId, new List<MatchAssignment>());
        }

        private static bool IsChangedToLiveMatchStatus(ChangedMatchSnapshot snapshot)
        {
            return IsCancelSuspendedPostponedStatus(snapshot.OldMatch.MatchStatus) && IsLiveStatus(snapshot.NewMatch.MatchStatus);
        }

        private static bool IsCancelSuspendedPostponedStatus(string matchStatus)
        {
            return matchStatus.IndexOf("CANCEL", StringComparison.CurrentCultureIgnoreCase) >= 0
              || matchStatus.IndexOf("SUSPENDED", StringComparison.CurrentCultureIgnoreCase) >= 0
              || matchStatus.IndexOf("POSTPONED", StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private static bool IsLiveStatus(string matchStatus)
        {
            return matchStatus.IndexOf("LIVE", StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        public static IEnumerable<ChangedValue> GetIndirectChangedValues(
            ChangedMatchSnapshot snapshot,
            List<ChangedValue> allChangedValues,
            List<Assignee> existingAssignees)
        {
            return existingAssignees
                .Select(t => allChangedValues.Where(x => !x.IsChangedToLiveMatchStatus)
                                             .Select(a => GetIndirectChangedValue(a, t)))
                .SelectMany(a => a);
        }

        public static ChangedValue GetIndirectChangedValue(ChangedValue changedValue, Assignee assignee)
        {
            return new ChangedValue
            {
                MessageTypeId = changedValue.MessageTypeId,
                BetTypeId = changedValue.BetTypeId,
                OldValue = changedValue.OldValue,
                NewValue = changedValue.NewValue,
                BetTypeName = changedValue.BetTypeName,
                MatchId = changedValue.MatchId,
                MatchNo = changedValue.MatchNo,
                League = changedValue.League,
                EventName = changedValue.EventName,
                AssigneeId = assignee.AssigneeId,
                AssigneeName = assignee.AssigneeName,
                AssignmentType = changedValue.AssignmentType,
                AssignmentNote = changedValue.AssignmentNote,
                IsIndirectChangedAssignment = true
            };
        }
    }
}
