namespace OWP.AssignmentUploadNotification.Models
{
    using System.Collections.Generic;

    public class ChangedValue : IEquatable<ChangedValue>
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

        public bool Equals(ChangedValue y)
        {
            if (y is null)
            {
                return false;
            }
            var condition1 = this.MatchId == y.MatchId && this.BetTypeId == y.BetTypeId && this.AssignmentType == y.AssignmentType;
            var condition2 = this.MessageTypeId == y.MessageTypeId && this.AssigneeId == y.AssigneeId && this.IsIndirectChangedAssignment == y.IsIndirectChangedAssignment;
            var condition3 = this.OldValue == y.OldValue && this.NewValue == y.NewValue && this.AssignmentNote == y.AssignmentNote;

            return condition1 && condition2 && condition3;
        }

        public override bool Equals(object obj) => Equals(obj as ChangedValue);
        public override int GetHashCode() => (MatchId, BetTypeId, MessageTypeId).GetHashCode();
    }
}