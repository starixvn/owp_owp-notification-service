using OWP.PreMatchNotification.Assignment.Services.MyAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.MyAssignment;

public class ChangedValueOuDetectorUnitTests
{
    [Fact]
    public void DetectChangeMainAssignment_ReturnChangedValue()
    {
        var ouDetector = new ChangedValueOuDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            Ou = "Sa",
            OuUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            Ou = "Starix15",
            OuUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "O/U",
                League = "League",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "O/U",
                League = "League",
                MessageTypeId = 202,
                ReceivedUserIds = new List<int>{8}
            }
        };
        ouDetector.DetectMain(oldSnapshot, newSnapshot, actual);
        DeepAssert.Equal(actual, expected);
    }

    [Fact]
    public void DetectChangeOtherAssignment_ReturnChangedValue()
    {
        var ouDetector = new ChangedValueOuDetectorImp();
        var oldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",
            IsDay = true,
            OtherOu = "Sa",
            OtherOuUserId = 8
        };

        var newSnapshot = new AssignmentSnapshot
        {
            OtherOu = "Starix15",
            OtherOuUserId = 121
        };

        var actual = new List<ChangedValue>();
        var expected = new List<ChangedValue>
        {
            new ()
            {
                BetTypeName = "O/U",
                League = "League",
                MessageTypeId = 201,
                ReceivedUserIds = new List<int>{121}
            },
            new ()
            {
                BetTypeName = "O/U",
                League = "League",
                MessageTypeId = 202,
                ReceivedUserIds = new List<int>{8}
            }
        };
        ouDetector.DetectOther(oldSnapshot, newSnapshot, actual);
        DeepAssert.Equal(actual, expected);
    }
}