using OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.OtherAssignment;

public class ChangedValueHdpDetectorUnitTests
{
    [Fact]
    public void DetectChangeMainAssignment_ReturnChangedValue()
    {
        var hdpDetector = new ChangedValueHdpDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            Hdp = "Sa",
            HdpUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            Hdp = "Starix15",
            HdpUserId = 121,
            IsDay = true
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                League = "League",
                BetTypeName = "HDP",
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
        var hdpDetector = new ChangedValueHdpDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            OtherHdp = "Sa",
            OtherHdpUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            OtherHdp = "Starix15",
            OtherHdpUserId = 121,
            IsDay = true
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                League = "League",
                BetTypeName = "HDP",
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