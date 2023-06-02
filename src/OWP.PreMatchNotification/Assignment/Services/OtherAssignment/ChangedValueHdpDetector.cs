using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

public interface ChangedValueHdpDetector
{
    void DetectMain(
      AssignmentSnapshot oldSnapshot,
      AssignmentSnapshot newSnapshot,
      IList<ChangedValue> changedValues,
      int receivedUserId);

    void DetectOther(
        AssignmentSnapshot oldSnapshot,
        AssignmentSnapshot newSnapshot,
        IList<ChangedValue> changedValues,
        int receivedUserId);
}

public class ChangedValueHdpDetectorImp : ChangedValueHdpDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues, int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.Hdp, newSnapshot.Hdp))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = new List<int> { receivedUserId },
                BetTypeName = Bettype.HDP,
                OldValue = oldSnapshot.Hdp,
                NewValue = newSnapshot.Hdp,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay)
            });
        }
    }

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues,
        int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.OtherHdp, newSnapshot.OtherHdp))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = new List<int> { receivedUserId },
                BetTypeName = Bettype.HDP,
                OldValue = oldSnapshot.OtherHdp,
                NewValue = newSnapshot.OtherHdp,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay)
            });
        }
    }
}