using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.MyAssignment;

public interface ChangedValueHdpDetector
{
    void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);
}

public class ChangedValueHdpDetectorImp : ChangedValueHdpDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
        => AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.Hdp,
            NewAssignment = newSnapshot.Hdp,
            BettypeName = Bettype.HDP,
            UserIdNewAssign = newSnapshot.HdpUserId,
            UserIdRemoved = oldSnapshot.HdpUserId
        }, changedValues);

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
        => AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.OtherHdp,
            NewAssignment = newSnapshot.OtherHdp,
            BettypeName = Bettype.HDP,
            UserIdNewAssign = newSnapshot.OtherHdpUserId,
            UserIdRemoved = oldSnapshot.OtherHdpUserId
        }, changedValues);
}