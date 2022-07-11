using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.MyAssignment;

public interface ChangedValueOuTakeOverDetector
{
    void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues);

    void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues);
}

public class ChangedValueOuTakeOverDetectorImp : ChangedValueOuTakeOverDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
        => AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.OuTakeOver,
            NewAssignment = newSnapshot.OuTakeOver,
            BettypeName = Bettype.OU_TAKEOVER,
            UserIdNewAssign = newSnapshot.OuTakeOverUserId,
            UserIdRemoved = oldSnapshot.OuTakeOverUserId
        }, changedValues);

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
        => AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.OtherOuTakeOver,
            NewAssignment = newSnapshot.OtherOuTakeOver,
            BettypeName = Bettype.OU_TAKEOVER,
            UserIdNewAssign = newSnapshot.OtherOuTakeOverUserId,
            UserIdRemoved = oldSnapshot.OtherOuTakeOverUserId
        }, changedValues);
}