namespace OWP.OtherSportNotification.Models;

public class ChangedValue
{
    public int UserId { get; set; }

    public string Username { get; set; }

    public int MessageTypeId { get; set; }

    public int NotificationSettingId { get; set; }

    public string NewValue { get; set; }

    public string OldValue { get; set; }

    public string MatchNo { get; set; }

    public string League { get; set; }

    public string EventName { get; set; }

    public DateTime EventDateTime { get; set; }

    public int OSMatchId { get; set; }

    public string BetTypeName { get; set; }

    public string AssignmentNote { get; set; }

    public int AssignmentType { get; set; }

    public bool IsIndirectChangedAssignment { get; set; }

    public bool IsWatcher { get; set; }

    public ChangedValue()
    {
        AssignmentNote = string.Empty;
    }
}

public class AssignmentChangedValueComparer : ChangedValue, IEqualityComparer<ChangedValue>
{
    public bool Equals(ChangedValue x, ChangedValue y)
    {
        var condition = y != null && x != null;
        var condition1 = condition && x.OSMatchId == y.OSMatchId && x.BetTypeName == y.BetTypeName && x.AssignmentType == y.AssignmentType;
        var condition2 = condition && x.MessageTypeId == y.MessageTypeId && x.UserId == y.UserId && x.IsIndirectChangedAssignment == y.IsIndirectChangedAssignment;
        var condition3 = condition && x.OldValue == y.OldValue && x.NewValue == y.NewValue && x.AssignmentNote == y.AssignmentNote;

        return condition1 && condition2 && condition3;
    }

    public int GetHashCode(ChangedValue obj)
    {
        return obj.OSMatchId * 17 + obj.MessageTypeId * 29;
    }
}
