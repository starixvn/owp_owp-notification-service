namespace OWP.AssignmentUploadNotification.Models
{
    using System.Collections.Generic;

    public class ChangedValue
    {
        public int AssigneeId { get; set; }

        public string AssigneeName { get; set; }

        public int MessageTypeId { get; set; }

        public int NotificationSettingId { get; set; }

        public string NewValue { get; set; }

        public string OldValue { get; set; }

        public string MatchNo { get; set; }

        public string League { get; set; }

        public string EventName { get; set; }

        public int MatchId { get; set; }

        public int BetTypeId { get; set; }

        public string BetTypeName { get; set; }

        public string AssignmentNote { get; set; }

        public int AssignmentType { get; set; }

        public bool IsWatcher { get; set; }

        public bool IsIndirectChangedAssignment { get; set; }

        public bool IsChangedToLiveMatchStatus { get; set; }

        public ChangedValue()
        {
            AssignmentNote = string.Empty;
        }
    }

    public class AssignmentChangedValueComparer : ChangedValue, IEqualityComparer<ChangedValue>
    {
        public bool Equals(ChangedValue x, ChangedValue y)
        {
            var condition1 = x.MatchId == y.MatchId && x.BetTypeId == y.BetTypeId && x.AssignmentType == y.AssignmentType;
            var condition2 = x.MessageTypeId == y.MessageTypeId && x.AssigneeId == y.AssigneeId && x.IsIndirectChangedAssignment == y.IsIndirectChangedAssignment;
            var condition3 = x.OldValue == y.OldValue && x.NewValue == y.NewValue && x.AssignmentNote == y.AssignmentNote;

            return condition1 && condition2 && condition3;
        }

        public int GetHashCode(ChangedValue obj)
        {
            return obj.MatchId * 17 + obj.MessageTypeId * 29;
        }
    }
}