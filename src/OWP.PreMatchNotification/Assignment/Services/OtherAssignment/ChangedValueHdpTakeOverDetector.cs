using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

public interface ChangedValueHdpTakeOverDetector
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

public class ChangedValueHdpTakeOverDetectorImp : ChangedValueHdpTakeOverDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues, int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.HdpTakeOver, newSnapshot.HdpTakeOver))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = [receivedUserId],
                BetTypeName = Bettype.HDP_TAKEOVER,
                OldValue = oldSnapshot.HdpTakeOver,
                NewValue = newSnapshot.HdpTakeOver,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay != null && newSnapshot.IsDay.Value)
            });
        }
    }

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues,
        int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.OtherHdpTakeOver, newSnapshot.OtherHdpTakeOver))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = [receivedUserId],
                BetTypeName = Bettype.HDP_TAKEOVER,
                OldValue = oldSnapshot.OtherHdpTakeOver,
                NewValue = newSnapshot.OtherHdpTakeOver,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay != null && newSnapshot.IsDay.Value)
            });
        }
    }
}