using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.MyAssignment;

public interface ChangedValueHdpTakeOverDetector
{
    void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);
}

public class ChangedValueHdpTakeOverDetectorImp : ChangedValueHdpTakeOverDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
        => AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.HdpTakeOver,
            NewAssignment = newSnapshot.HdpTakeOver,
            BettypeName = Bettype.HDP_TAKEOVER,
            UserIdNewAssign = newSnapshot.HdpTakeOverUserId,
            UserIdRemoved = oldSnapshot.HdpTakeOverUserId
        }, changedValues);

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
        => AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.OtherHdpTakeOver,
            NewAssignment = newSnapshot.OtherHdpTakeOver,
            BettypeName = Bettype.HDP_TAKEOVER,
            UserIdNewAssign = newSnapshot.OtherHdpTakeOverUserId,
            UserIdRemoved = oldSnapshot.OtherHdpTakeOverUserId
        }, changedValues);
}