using OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.OtherAssignment;

public class ChangedValueOuTakeOverDetectorUnitTests
{
    [Fact]
    public void DetectChangeMainAssignment_ReturnChangedValue()
    {
        var hdpDetector = new ChangedValueOuTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            OuTakeOver = "Sa",
            OuTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            OuTakeOver = "Starix15",
            OuTakeOverUserId = 121,
            IsDay = true
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                League = "League",
                BetTypeName = "O/U T.OVER",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = new List<int>{121 },
                MessageTypeId = 203
            }
        };
        hdpDetector.DetectMain(oldSnapshot, newSnapshot, actual, 121);
        DeepAssert.Equal(actual, expected);
    }

    [Fact]
    public void DetectChangeOtherAssignment_ReturnChangedValue()
    {
        var hdpDetector = new ChangedValueOuTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            OtherOuTakeOver = "Sa",
            OtherOuTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            OtherOuTakeOver = "Starix15",
            OtherOuTakeOverUserId = 121,
            IsDay = true
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                League = "League",
                BetTypeName = "O/U T.OVER",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = new List<int>{121 },
                MessageTypeId = 203
            }
        };
        hdpDetector.DetectOther(oldSnapshot, newSnapshot, actual, 121);
        DeepAssert.Equal(actual, expected);
    }
}