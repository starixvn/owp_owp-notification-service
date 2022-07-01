using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Services.MyAssignment
{
    public interface MyAssignmentGenerator
    {
        IEnumerable<ChangedValue> Create(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot);
    }

    public class MyAssignmentGeneratorImp : MyAssignmentGenerator
    {
        private readonly MyAssignmentDetector myAssignmentDetector;

        public MyAssignmentGeneratorImp(MyAssignmentDetector myAssignmentDetector)
        {
            this.myAssignmentDetector = myAssignmentDetector;
        }

        public IEnumerable<ChangedValue> Create(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
        {
            var changedValues = new List<ChangedValue>();
            myAssignmentDetector.DetectChangeAssignmentHdp(oldSnapshot, newSnapshot, changedValues);
            myAssignmentDetector.DetectChangeAssignmentOu(oldSnapshot, newSnapshot, changedValues);
            myAssignmentDetector.DetectChangeAssignmentHdpTOver(oldSnapshot, newSnapshot, changedValues);
            myAssignmentDetector.DetectChangeAssignmentOuTOver(oldSnapshot, newSnapshot, changedValues);
            myAssignmentDetector.DetectChangeAssignmentOtherHdp(oldSnapshot, newSnapshot, changedValues);
            myAssignmentDetector.DetectChangeAssignmentOtherOu(oldSnapshot, newSnapshot, changedValues);
            myAssignmentDetector.DetectChangeAssignmentOtherHdpTOver(oldSnapshot, newSnapshot, changedValues);
            myAssignmentDetector.DetectChangeAssignmentOtherOuTOver(oldSnapshot, newSnapshot, changedValues);

            return changedValues.Where(value => !string.IsNullOrWhiteSpace(value.League));
        }
    }
}