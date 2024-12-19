using OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.OtherAssignment;

public class ChangedValueHdpTakeOverDetectorUnitTests
{
    [Fact]
    public void DetectChangeMainAssignment_ReturnChangedValue()
    {
        var hdpDetector = new ChangedValueHdpTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            HdpTakeOver = "Sa",
            HdpTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            HdpTakeOver = "Starix15",
            HdpTakeOverUserId = 121,
            IsDay = true
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                League = "League",
                BetTypeName = "HDP T.OVER",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = [121],
                MessageTypeId = 203
            }
        };
        hdpDetector.DetectMain(oldSnapshot, newSnapshot, actual, 121);
        DeepAssert.Equal(actual, expected);
    }

    [Fact]
    public void DetectChangeOtherAssignment_ReturnChangedValue()
    {
        var hdpDetector = new ChangedValueHdpTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            OtherHdpTakeOver = "Sa",
            OtherHdpTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            OtherHdpTakeOver = "Starix15",
            OtherHdpTakeOverUserId = 121,
            IsDay = true
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                League = "League",
                BetTypeName = "HDP T.OVER",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = [121],
                MessageTypeId = 203
            }
        };
        hdpDetector.DetectOther(oldSnapshot, newSnapshot, actual, 121);
        DeepAssert.Equal(actual, expected);
    }
}