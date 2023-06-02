using OWP.PreMatchNotification.Assignment.Services.MyAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.MyAssignment;

public class ChangedValueHdpTakeOverDetectorUnitTests
{
    [Fact]
    public void DetectChangeMainAssignment_ReturnChangedValue()
    {
        var hdpTakeOverDetector = new ChangedValueHdpTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            HdpTakeOver = "Sa",
            HdpTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            HdpTakeOver = "Starix15",
            HdpTakeOverUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "HDP T.OVER",
                League = "League",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "HDP T.OVER",
                League = "League",
                MessageTypeId = 202,
                ReceivedUserIds = new List<int>{8}
            }
        };
        hdpTakeOverDetector.DetectMain(oldSnapshot, newSnapshot, actual);
        DeepAssert.Equal(actual, expected);
    }

    [Fact]
    public void DetectChangeOtherAssignment_ReturnChangedValue()
    {
        var hdpTakeOverDetector = new ChangedValueHdpTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            OtherHdpTakeOver = "Sa",
            OtherHdpTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            OtherHdpTakeOver = "Starix15",
            OtherHdpTakeOverUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "HDP T.OVER",
                League = "League",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "HDP T.OVER",
                League = "League",
                MessageTypeId = 202,
                ReceivedUserIds = new List<int>{8}
            }
        };
        hdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, actual);
        DeepAssert.Equal(actual, expected);
    }
}