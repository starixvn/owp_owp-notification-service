using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.MyAssignment;

public interface MyAssignmentDetector
{
    void DetectChangeAssignmentHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentOtherHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentOtherOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentOtherHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);

    void DetectChangeAssignmentOtherOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot, List<ChangedValue> changedValues);
}

public class MyAssignmentDetectorImp(
    ChangedValueHdpDetector changedValueHdpDetector,
    ChangedValueHdpTakeOverDetector changedValueHdpTakeOverDetector,
    ChangedValueOuDetector changedValueOuDetector,
    ChangedValueOuTakeOverDetector changedValueOuTakeOverDetector)
    : MyAssignmentDetector
{
    public void DetectChangeAssignmentHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueHdpDetector.DetectMain(oldSnapshot, newSnapshot, changedValues);

    public void DetectChangeAssignmentOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueOuDetector.DetectMain(oldSnapshot, newSnapshot, changedValues);

    public void DetectChangeAssignmentHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueHdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues);

    public void DetectChangeAssignmentOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueOuTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, changedValues);

    public void DetectChangeAssignmentOtherHdp(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueHdpDetector.DetectOther(oldSnapshot, newSnapshot, changedValues);

    public void DetectChangeAssignmentOtherOu(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueOuDetector.DetectOther(oldSnapshot, newSnapshot, changedValues);

    public void DetectChangeAssignmentOtherHdpTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueHdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues);

    public void DetectChangeAssignmentOtherOuTOver(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot,
        List<ChangedValue> changedValues)
        => changedValueOuTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, changedValues);
}