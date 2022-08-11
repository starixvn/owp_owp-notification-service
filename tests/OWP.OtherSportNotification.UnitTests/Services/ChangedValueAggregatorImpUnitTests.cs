using AutoMapper;

namespace OWP.OtherSportNotification.UnitTests.Services;
public class ChangedValueAggregatorImpUnitTests
{
    private readonly IMapper mapper;

    public ChangedValueAggregatorImpUnitTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ChangedValue, ChangedValue>();
        });

        mapper = configuration.CreateMapper();
    }

    [Fact]
    public void GetNonAggregatedChangedValues_Always_ReturnsAllDirectOrNonAssignmentValues()
    {
        // Arrange
        var changedValues = GetChangedValues();
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.GetNonAggregatedChangedValues(changedValues);

        // Assert
        Assert.Equal(5, actual.Count());
        Assert.Contains(actual, item => item.OSMatchId == 10 & item.BetTypeName == "1H (HDP)");
        Assert.Contains(actual, item => item.OSMatchId == 10 & item.BetTypeName == "1Q & 3Q (HDP)");
        Assert.Contains(actual, item => item.OSMatchId == 10 & item.BetTypeName == "FT (OU/OE)");
        Assert.Contains(actual, item => item.OSMatchId == 10 & item.BetTypeName == "1H (OU/OE)");
        Assert.Contains(actual, item => item.OSMatchId == 40);
    }

    [Fact]
    public void GetIndirectOtherAssignmentChangedValues_Always_ReturnsAllOtherAssignmentValuesOfDirectOrWatcher()
    {
        // Arrange
        var changedValues = GetChangedValues();
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.GetIndirectOtherAssignmentChangedValues(changedValues);

        // Assert
        Assert.Equal(2, actual.Count());
        Assert.Contains(actual, item => item.OSMatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentInsert);
        Assert.Contains(actual, item => item.OSMatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentDelete);
    }

    [Fact]
    public void GetIndirectMainAssignmentChangedValues_Always_ReturnsAllMainAssignmentValuesOfDirectOrWatcher()
    {
        // Arrange
        var changedValues = GetChangedValues();
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.GetIndirectMainAssignmentChangedValues(changedValues);

        // Assert
        Assert.Equal(4, actual.Count());
        Assert.Contains(actual, item => item.OSMatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert && item.BetTypeName == "1H (OU)");
        Assert.Contains(actual, item => item.OSMatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert && item.BetTypeName == "1Q & 3Q (OU/OE)");
        Assert.Contains(actual, item => item.OSMatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete && item.BetTypeName == "1Q & 3Q (OU)");
    }

    [Fact]
    public void GetAggregatedValue_Always_UpdateOldAndNewValueAndMessageType()
    {
        // Arrange
        var deletingChangedValue = GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, 1, "1H (HDP)");
        var insertingChangedValue = GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, 1, "1H (HDP)");
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.GetAggregatedValueForDoubleAssignment(deletingChangedValue, insertingChangedValue);

        // Assert
        Assert.Equal("NewValue", actual.NewValue);
        Assert.Equal("OldValue", actual.OldValue);
        Assert.Equal((int)MsgTemplateCode.AssignmentUpdate, actual.MessageTypeId);
    }

    [Fact]
    public void Aggregate_Always_UpdateMessageTypeForIndirectAndWatcherAssignment()
    {
        // Arrange
        var changedValues = GetChangedValues();
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.Aggregate(changedValues);

        // Assert
        Assert.Equal(10, actual.Count);

        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentInsert));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentDelete));

        Assert.Equal(5, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentUpdate));

        Assert.Single(actual, item => item.MessageTypeId == (int)MsgTemplateCode.EditWB);
    }

    private List<ChangedValue> GetChangedValues()
    {
        const int mainAssignment = 1;
        const int otherAssignment = 2;
        var changedValues = new List<ChangedValue>
            {
                GetDirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, "1H (HDP)"),
                GetDirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, "1Q & 3Q (HDP)"),
                GetDirectAssignment((int)MsgTemplateCode.OtherAssignmentInsert, otherAssignment, "FT (OU/OE)"),
                GetDirectAssignment((int)MsgTemplateCode.OtherAssignmentDelete, otherAssignment, "1H (OU/OE)"),

                GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, "1H (OU)"),
                GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, "1H (OU)"),
                GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, "1Q & 3Q (OU/OE)"),
                GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, "1Q & 3Q (OU)"),
                GetIndirectAssignment((int)MsgTemplateCode.OtherAssignmentInsert, otherAssignment, "1Q (HDP)"),
                GetIndirectAssignment((int)MsgTemplateCode.OtherAssignmentDelete, otherAssignment, "1Q (OU)"),

                GetChangedInfoValue()
            };

        return changedValues;
    }

    private static ChangedValue GetDirectAssignment(int messageTypeId, int assignmentType, string BetTypeName)
    {
        return new ChangedValue
        {
            MessageTypeId = messageTypeId,
            BetTypeName = BetTypeName,
            AssignmentType = assignmentType,
            OSMatchId = 10,
            UserId = 100,
            IsIndirectChangedAssignment = false
        };
    }

    private static ChangedValue GetIndirectAssignment(int messageTypeId, int assignmentType, string BetTypeName)
    {
        return new ChangedValue
        {
            MessageTypeId = messageTypeId,
            BetTypeName = BetTypeName,
            AssignmentType = assignmentType,
            OSMatchId = 20,
            UserId = 200,
            IsIndirectChangedAssignment = true,
            OldValue = messageTypeId == (int)MsgTemplateCode.AssignmentInsert ? string.Empty : "OldValue",
            NewValue = messageTypeId == (int)MsgTemplateCode.AssignmentDelete ? string.Empty : "NewValue"
        };
    }

    private static ChangedValue GetChangedInfoValue()
    {
        return new ChangedValue
        {
            MessageTypeId = (int)MsgTemplateCode.EditWB,
            OSMatchId = 40,
            UserId = 400,
            IsIndirectChangedAssignment = false
        };
    }
}
