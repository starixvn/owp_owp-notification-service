using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

public interface OtherAssignmentGenerator
{
    IEnumerable<ChangedValue> Create(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot);
}

public class OtherAssignmentGeneratorImp(OtherAssignmentDetector otherAssignmentDetector) : OtherAssignmentGenerator
{
    public IEnumerable<ChangedValue> Create(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        var changedValues = new List<ChangedValue>();
        otherAssignmentDetector.DetectChangeOtherAssignmentExceptHdp(oldSnapshot, newSnapshot, changedValues);
        otherAssignmentDetector.DetectChangeAssignmentExceptOu(oldSnapshot, newSnapshot, changedValues);
        otherAssignmentDetector.DetectChangeAssignmentExceptHdpTOver(oldSnapshot, newSnapshot, changedValues);
        otherAssignmentDetector.DetectChangeAssignmentExceptOuTOver(oldSnapshot, newSnapshot, changedValues);

        otherAssignmentDetector.DetectChangeAssignmentExceptOtherHdp(oldSnapshot, newSnapshot, changedValues);
        otherAssignmentDetector.DetectChangeAssignmentExceptOtherOu(oldSnapshot, newSnapshot, changedValues);
        otherAssignmentDetector.DetectChangeAssignmentExceptOtherHdpTOver(oldSnapshot, newSnapshot, changedValues);
        otherAssignmentDetector.DetectChangeAssignmentExceptOtherOuTOver(oldSnapshot, newSnapshot, changedValues);

        return changedValues;
    }
}