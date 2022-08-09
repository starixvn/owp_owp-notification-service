namespace OWP.PreMatchNotification.UnitTests.Assignment.Helper;

public class AssignmentHelperUnitTests
{
    [Fact]
    public void DetectChangedValueNewAssignment_NoChangeAssignment_ReturnDefaultChangeValue()
    {
        var myAssignmentDetectorModel = BuildMyAssignmentDetectorModel();
        var changedValues = new List<ChangedValue>();
        AssignmentHelper.DetectChangedValueNewAssignment(myAssignmentDetectorModel, changedValues);
        var changedValuesExpected = new List<ChangedValue>();
        DeepAssert.Equal(changedValuesExpected, changedValues);
    }

    [Fact]
    public void DetectChangedValueNewAssignment_ChangeAssignment_IsDay_ReturnChangeValue()
    {
        var myAssignmentDetectorModel = BuildMyAssignmentDetectorModel();
        myAssignmentDetectorModel.NewAssignment = "starix15";
        myAssignmentDetectorModel.UserIdNewAssign = 123;

        var changedValues = new List<ChangedValue>();
        AssignmentHelper.DetectChangedValueNewAssignment(myAssignmentDetectorModel, changedValues);
        var changedValuesExpected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 201,
                BetTypeName = "Hdp",
                League = "VLeague",
                ReceivedUserIds = new List<int>{123}
            }
        };
        DeepAssert.Equal(changedValuesExpected, changedValues);
    }

    [Fact]
    public void DetectChangedValueNewAssignment_ChangeAssignment_IsNight_ReturnChangeValue()
    {
        var myAssignmentDetectorModel = BuildMyAssignmentDetectorModel();
        myAssignmentDetectorModel.NewAssignment = "starix15";
        myAssignmentDetectorModel.UserIdNewAssign = 123;
        myAssignmentDetectorModel.IsDay = false;

        var changedValues = new List<ChangedValue>();
        AssignmentHelper.DetectChangedValueNewAssignment(myAssignmentDetectorModel, changedValues);
        var changedValuesExpected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 204,
                BetTypeName = "Hdp",
                League = "VLeague",
                ReceivedUserIds = new List<int>{123}
            }
        };
        DeepAssert.Equal(changedValuesExpected, changedValues);
    }

    [Fact]
    public void DetectChangedValueDeleteAssignment_NoChangeAssignment_ReturnDefaultChangeValue()
    {
        var myAssignmentDetectorModel = BuildMyAssignmentDetectorModel();
        var changedValues = new List<ChangedValue>();
        AssignmentHelper.DetectChangedValueDeleteAssignment(myAssignmentDetectorModel, changedValues);
        var changedValuesExpected = new List<ChangedValue>();
        DeepAssert.Equal(changedValuesExpected, changedValues);
    }

    [Fact]
    public void DetectChangedValueDeleteAssignment_ChangeAssignment_IsDay_ReturnChangeValue()
    {
        var myAssignmentDetectorModel = BuildMyAssignmentDetectorModel();
        myAssignmentDetectorModel.OldAssignment = "starix15";
        myAssignmentDetectorModel.UserIdRemoved = 123;

        var changedValues = new List<ChangedValue>();
        AssignmentHelper.DetectChangedValueDeleteAssignment(myAssignmentDetectorModel, changedValues);
        var changedValuesExpected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 202,
                BetTypeName = "Hdp",
                League = "VLeague",
                ReceivedUserIds = new List<int>{123}
            }
        };
        DeepAssert.Equal(changedValuesExpected, changedValues);
    }

    [Fact]
    public void DetectChangedValueDeleteAssignment_ChangeAssignment_IsNight_ReturnChangeValue()
    {
        var myAssignmentDetectorModel = BuildMyAssignmentDetectorModel();
        myAssignmentDetectorModel.OldAssignment = "starix15";
        myAssignmentDetectorModel.UserIdRemoved = 123;
        myAssignmentDetectorModel.IsDay = false;

        var changedValues = new List<ChangedValue>();
        AssignmentHelper.DetectChangedValueDeleteAssignment(myAssignmentDetectorModel, changedValues);
        var changedValuesExpected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 205,
                BetTypeName = "Hdp",
                League = "VLeague",
                ReceivedUserIds = new List<int>{123}
            }
        };
        DeepAssert.Equal(changedValuesExpected, changedValues);
    }

    private MyAssignmentDetectorModel BuildMyAssignmentDetectorModel()
        => new()
        {
            LeagueName = "VLeague",

            OldAssignment = "sa",

            NewAssignment = "sa",

            BettypeName = "Hdp",

            UserIdRemoved = 8,

            UserIdNewAssign = 8,

            IsDay = true
        };
}