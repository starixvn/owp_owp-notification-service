using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

public interface OtherAssignmentDetector
{
    void DetectChangeOtherAssignmentExceptHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentExceptOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentExceptHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentExceptOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentExceptOtherHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentExceptOtherOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentExceptOtherHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentExceptOtherOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);
}

public class OtherAssignmentDetectorImp(
    ChangedValueHdpDetector changedValueHdpDetector,
    ChangedValueHdpTakeOverDetector changedValueHdpTakeOverDetector,
    ChangedValueOuDetector changedValueOuDetector,
    ChangedValueOuTakeOverDetector changedValueOuTakeOverDetector)
    : OtherAssignmentDetector
{
    public void DetectChangeOtherAssignmentExceptHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.Hdp, newSnapshot.Hdp) || string.IsNullOrWhiteSpace(newSnapshot.Hdp))
        {
            return;
        }

        var userId = newSnapshot.HdpUserId ?? 0;
        changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
    }

    public void DetectChangeAssignmentExceptHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.HdpTakeOver, newSnapshot.HdpTakeOver) ||
            string.IsNullOrWhiteSpace(newSnapshot.HdpTakeOver))
        {
            return;
        }

        var userId = newSnapshot.HdpTakeOverUserId ?? 0;
        changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
    }

    public void DetectChangeAssignmentExceptOtherHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.OtherHdp, newSnapshot.OtherHdp) ||
            string.IsNullOrWhiteSpace(newSnapshot.OtherHdp))
        {
            return;
        }

        var userId = newSnapshot.OtherHdpUserId ?? 0;
        changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
    }

    public void DetectChangeAssignmentExceptOtherHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.OtherHdpTakeOver, newSnapshot.OtherHdpTakeOver) ||
            string.IsNullOrWhiteSpace(newSnapshot.OtherHdpTakeOver))
        {
            return;
        }

        var userId = newSnapshot.OtherHdpTakeOverUserId ?? 0;
        changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
    }

    public void DetectChangeAssignmentExceptOtherOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.OtherOu, newSnapshot.OtherOu) ||
            string.IsNullOrWhiteSpace(newSnapshot.OtherOu))
        {
            return;
        }

        var userId = newSnapshot.OtherOuUserId ?? 0;
        changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
    }

    public void DetectChangeAssignmentExceptOtherOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.OtherOuTakeOver, newSnapshot.OtherOuTakeOver) ||
            string.IsNullOrWhiteSpace(newSnapshot.OtherOuTakeOver))
        {
            return;
        }

        var userId = newSnapshot.OtherOuTakeOverUserId ?? 0;
        changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
    }

    public void DetectChangeAssignmentExceptOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.Ou, newSnapshot.Ou) || string.IsNullOrWhiteSpace(newSnapshot.Ou))
        {
            return;
        }
        var userId = newSnapshot.OuUserId ?? 0;
        changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
    }

    public void DetectChangeAssignmentExceptOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues)
    {
        if (!string.Equals(oldSnapshot.OuTakeOver, newSnapshot.OuTakeOver) ||
            string.IsNullOrWhiteSpace(newSnapshot.OuTakeOver))
        {
            return;
        }

        var userId = newSnapshot.OuTakeOverUserId ?? 0;
        changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues, userId);
        changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues, userId);
    }
}