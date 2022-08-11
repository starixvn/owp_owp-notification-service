using OWP.PreMatchNotification.Assignment.Services.MyAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.MyAssignment;

public class ChangedValueOuTakeOverDetectorUnitTests
{
    [Fact]
    public void DetectChangeMainAssignment_ReturnChangedValue()
    {
        var hdpTakeOverDetector = new ChangedValueOuTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "VLeague",
            IsDay = true,
            OuTakeOver = "Sa",
            OuTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            OuTakeOver = "Starix15",
            OuTakeOverUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "O/U T.OVER",
                League = "VLeague",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "O/U T.OVER",
                League = "VLeague",
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
        var hdpTakeOverDetector = new ChangedValueOuTakeOverDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "VLeague",
            IsDay = true,
            OtherOuTakeOver = "Sa",
            OtherOuTakeOverUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            OtherOuTakeOver = "Starix15",
            OtherOuTakeOverUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "O/U T.OVER",
                League = "VLeague",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "O/U T.OVER",
                League = "VLeague",
                MessageTypeId = 202,
                ReceivedUserIds = new List<int>{8}
            }
        };
        hdpTakeOverDetector.DetectOther(oldSnapshot, newSnapshot, actual);
        DeepAssert.Equal(actual, expected);
    }
}