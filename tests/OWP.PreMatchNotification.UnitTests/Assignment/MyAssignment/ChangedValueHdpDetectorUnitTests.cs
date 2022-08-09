using OWP.PreMatchNotification.Assignment.Services.MyAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.MyAssignment;

public class ChangedValueHdpDetectorUnitTests
{
    [Fact]
    public void DetectChangeMainAssignment_ReturnChangedValue()
    {
        var hdpDetector = new ChangedValueHdpDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "VLeague",
            IsDay = true,
            Hdp = "Sa",
            HdpUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            Hdp = "Starix15",
            HdpUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "HDP",
                League = "VLeague",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "HDP",
                League = "VLeague",
                MessageTypeId = 202,
                ReceivedUserIds = new List<int>{8}
            }
        };
        hdpDetector.DetectMain(oldSnapshot, newSnapshot, actual);
        DeepAssert.Equal(actual, expected);
    }

    [Fact]
    public void DetectChangeOtherAssignment_ReturnChangedValue()
    {
        var hdpDetector = new ChangedValueHdpDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "VLeague",
            IsDay = true,
            OtherHdp = "Sa",
            OtherHdpUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            OtherHdp = "Starix15",
            OtherHdpUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "HDP",
                League = "VLeague",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "HDP",
                League = "VLeague",
                MessageTypeId = 202,
                ReceivedUserIds = new List<int>{8}
            }
        };
        hdpDetector.DetectOther(oldSnapshot, newSnapshot, actual);
        DeepAssert.Equal(actual, expected);
    }
}