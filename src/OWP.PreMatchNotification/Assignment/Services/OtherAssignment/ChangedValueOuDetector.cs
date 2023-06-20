using OWP.PreMatchNotification.Assignment.Constants;
using OWP.PreMatchNotification.Assignment.Helper;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

public interface ChangedValueOuDetector
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

public class ChangedValueOuDetectorImp : ChangedValueOuDetector
{
    public void DetectMain(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues, int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.Ou, newSnapshot.Ou))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = new List<int> { receivedUserId },
                BetTypeName = Bettype.OU,
                OldValue = oldSnapshot.Ou,
                NewValue = newSnapshot.Ou,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay != null && newSnapshot.IsDay.Value)
            });
        }
    }

    public void DetectOther(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, IList<ChangedValue> changedValues,
        int receivedUserId)
    {
        if (!string.Equals(oldSnapshot.OtherOu, newSnapshot.OtherOu))
        {
            changedValues.Add(new ChangedValue
            {
                League = newSnapshot.LeagueName,
                ReceivedUserIds = new List<int> { receivedUserId },
                BetTypeName = Bettype.OU,
                OldValue = oldSnapshot.OtherOu,
                NewValue = newSnapshot.OtherOu,
                MessageTypeId = AssignmentHelper.DetectMessageType(newSnapshot.IsDay != null && newSnapshot.IsDay.Value)
            });
        }
    }
}