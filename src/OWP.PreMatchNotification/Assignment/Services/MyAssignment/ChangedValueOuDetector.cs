using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.MyAssignment;

public interface ChangedValueOuDetector
{
    void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues);

    void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues);
}

public class ChangedValueOuDetectorImp : ChangedValueOuDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
    {
        AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.Ou,
            NewAssignment = newSnapshot.Ou,
            BettypeName = Bettype.OU,
            UserIdNewAssign = newSnapshot.OuUserId,
            UserIdRemoved = oldSnapshot.OuUserId
        }, changedValues);
    }

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
        => AssignmentHelper.DetectChangedValue(new MyAssignmentDetectorModel
        {
            LeagueName = oldSnapshot.LeagueName,
            IsDay = oldSnapshot.IsDay,
            OldAssignment = oldSnapshot.OtherOu,
            NewAssignment = newSnapshot.OtherOu,
            BettypeName = Bettype.OU,
            UserIdNewAssign = newSnapshot.OtherOuUserId,
            UserIdRemoved = oldSnapshot.OtherOuUserId
        }, changedValues);
}