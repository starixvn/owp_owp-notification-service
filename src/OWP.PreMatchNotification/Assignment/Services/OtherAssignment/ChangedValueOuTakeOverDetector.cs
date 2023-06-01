using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

public interface ChangedValueOuTakeOverDetector
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

public class ChangedValueOuTakeOverDetectorImp : ChangedValueOuTakeOverDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues, int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.OuTakeOver, newSnapshot.OuTakeOver))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = new List<int> { receivedUserId },
                BetTypeName = Bettype.OU_TAKEOVER,
                OldValue = oldSnapshot.OuTakeOver,
                NewValue = newSnapshot.OuTakeOver,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay!.Value)
            });
        }
    }

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues,
        int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.OtherOuTakeOver, newSnapshot.OtherOuTakeOver))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = new List<int> { receivedUserId },
                BetTypeName = Bettype.OU_TAKEOVER,
                OldValue = oldSnapshot.OtherOuTakeOver,
                NewValue = newSnapshot.OtherOuTakeOver,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay.Value)
            });
        }
    }
}