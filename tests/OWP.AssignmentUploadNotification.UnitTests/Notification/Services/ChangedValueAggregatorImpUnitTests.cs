using AutoMapper;
using OWP.AssignmentUploadNotification.Constants;
using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using Xunit;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

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

        // Act
        var actual = ChangedValueAggregatorImp.GetNonAggregatedChangedValues(changedValues).ToList();

        // Assert
        Assert.Equal(5, actual.Count);
        Assert.Contains(actual, item => item.MatchId == 10 & item.BetTypeId == 1);
        Assert.Contains(actual, item => item.MatchId == 10 & item.BetTypeId == 2);
        Assert.Contains(actual, item => item.MatchId == 10 & item.BetTypeId == 3);
        Assert.Contains(actual, item => item.MatchId == 10 & item.BetTypeId == 4);
        Assert.Contains(actual, item => item.MatchId == 40);
    }

    [Fact]
    public void GetIndirectOtherAssignmentChangedValues_Always_ReturnsAllOtherAssignmentValuesOfDirectOrWatcher()
    {
        // Arrange
        var changedValues = GetChangedValues();

        // Act
        var actual = ChangedValueAggregatorImp.GetIndirectOtherAssignmentChangedValues(changedValues).ToList();

        // Assert
        Assert.Equal(4, actual.Count);
        Assert.Contains(actual, item => item.MatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentInsert);
        Assert.Contains(actual, item => item.MatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentDelete);
        Assert.Contains(actual, item => item.MatchId == 30 & item.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentInsert);
        Assert.Contains(actual, item => item.MatchId == 30 & item.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentDelete);
    }

    [Fact]
    public void GetIndirectMainAssignmentChangedValues_Always_ReturnsAllMainAssignmentValuesOfDirectOrWatcher()
    {
        // Arrange
        var changedValues = GetChangedValues();

        // Act
        var actual = ChangedValueAggregatorImp.GetIndirectMainAssignmentChangedValues(changedValues).ToList();

        // Assert
        Assert.Equal(8, actual.Count);
        Assert.Contains(actual, item => item.MatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert && item.BetTypeId == 5);
        Assert.Contains(actual, item => item.MatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete && item.BetTypeId == 5);
        Assert.Contains(actual, item => item.MatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert && item.BetTypeId == 6);
        Assert.Contains(actual, item => item.MatchId == 20 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete && item.BetTypeId == 7);

        Assert.Contains(actual, item => item.MatchId == 30 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert && item.BetTypeId == 10);
        Assert.Contains(actual, item => item.MatchId == 30 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete && item.BetTypeId == 10);
        Assert.Contains(actual, item => item.MatchId == 30 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert && item.BetTypeId == 11);
        Assert.Contains(actual, item => item.MatchId == 30 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete && item.BetTypeId == 12);
    }


    [Fact]
    public void GetAggregatedValue_Always_UpdateOldAndNewValueAndMessageType()
    {
        // Arrange
        var deletingChangedValue = GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, 1, 1);
        var insertingChangedValue = GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, 1, 1);
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.GetAggregatedValueForDoubleAssignment(deletingChangedValue, insertingChangedValue);

        // Assert
        Assert.Equal("NewValue", actual.NewValue);
        Assert.Equal("OldValue", actual.OldValue);
        Assert.Equal((int)MsgTemplateCode.ChangeOfAssignment, actual.MessageTypeId);
    }

    [Fact]
    public void GetAggregatedValueForIndirectMainAssignments_Always_UpdateMessageTypeAndMergeChangedValue()
    {
        // Arrange
        var changedValues = GetMainAssignmentChangedValuesOfIndirectAndWatcher();
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.GetAggregatedValueForIndirectMainAssignments(changedValues).ToList();

        // Assert
        Assert.Equal(6, actual.Count);
        Assert.All(actual, item => Assert.Equal((int)MsgTemplateCode.ChangeOfAssignment, item.MessageTypeId));

        Assert.Contains(actual, item => item.MatchId == 20 && item.BetTypeId == 5 && item.OldValue == "OldValue" && item.NewValue == "NewValue");
        Assert.Contains(actual, item => item.MatchId == 20 && item.BetTypeId == 6 && item.OldValue == "" && item.NewValue == "NewValue");
        Assert.Contains(actual, item => item.MatchId == 20 && item.BetTypeId == 7 && item.OldValue == "OldValue" && item.NewValue == "");

        Assert.Contains(actual, item => item.MatchId == 30 && item.BetTypeId == 10 && item.OldValue == "OldValue" && item.NewValue == "NewValue");
        Assert.Contains(actual, item => item.MatchId == 30 && item.BetTypeId == 11 && item.OldValue == "" && item.NewValue == "NewValue");
        Assert.Contains(actual, item => item.MatchId == 30 && item.BetTypeId == 12 && item.OldValue == "OldValue" && item.NewValue == "");
    }

    [Fact]
    public void GetAggregatedValueForIndirectOtherAssignments_Always_UpdateMessageType()
    {
        // Arrange
        var changedValues = GetOtherAssignmentChangedValuesOfIndirectAndWatcher();

        // Act
        var actual = ChangedValueAggregatorImp.GetAggregatedValueForIndirectOtherAssignments(changedValues).ToList();

        // Assert
        Assert.Equal(4, actual.Count);
        Assert.All(actual, item => Assert.Equal((int)MsgTemplateCode.ChangeOfAssignment, item.MessageTypeId));
    }

    [Fact]
    public void Aggregate_Always_UpdateMessageTypeForIndirectAndWatcherAssignment()
    {
        // Arrange
        var changedValues = GetChangedValues();
        var service = new ChangedValueAggregatorImp(mapper);

        // Act
        var actual = service.Aggregate(changedValues).ToList();

        // Assert
        Assert.Equal(15, actual.Count);

        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentInsert));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentDelete));

        Assert.Equal(10, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.ChangeOfAssignment));

        Assert.Single(actual, item => item.MessageTypeId == (int)MsgTemplateCode.EditWB);
    }

    private static List<ChangedValue> GetChangedValues()
    {
        const int mainAssignment = 1;
        const int otherAssignment = 2;
        var changedValues = new List<ChangedValue>
        {
            GetDirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 1),
            GetDirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 2),
            GetDirectAssignment((int)MsgTemplateCode.OtherAssignmentInsert, otherAssignment, 3),
            GetDirectAssignment((int)MsgTemplateCode.OtherAssignmentDelete, otherAssignment, 4),

            GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 5),
            GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 5),
            GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 6),
            GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 7),
            GetIndirectAssignment((int)MsgTemplateCode.OtherAssignmentInsert, otherAssignment, 8),
            GetIndirectAssignment((int)MsgTemplateCode.OtherAssignmentDelete, otherAssignment, 9),

            GetWatcherAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 10),
            GetWatcherAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 10),
            GetWatcherAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 11),
            GetWatcherAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 12),
            GetWatcherAssignment((int)MsgTemplateCode.OtherAssignmentInsert, otherAssignment, 13),
            GetWatcherAssignment((int)MsgTemplateCode.OtherAssignmentDelete, otherAssignment, 14),

            GetChangedInfoValue()
        };

        return changedValues;
    }

    private List<ChangedValue> GetMainAssignmentChangedValuesOfIndirectAndWatcher()
    {
        const int mainAssignment = 1;
        var changedValues = new List<ChangedValue>
        {
            GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 5),
            GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 5),
            GetIndirectAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 6),
            GetIndirectAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 7),

            GetWatcherAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 10),
            GetWatcherAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 10),
            GetWatcherAssignment((int)MsgTemplateCode.AssignmentInsert, mainAssignment, 11),
            GetWatcherAssignment((int)MsgTemplateCode.AssignmentDelete, mainAssignment, 12)
        };

        return changedValues;
    }

    private List<ChangedValue> GetOtherAssignmentChangedValuesOfIndirectAndWatcher()
    {
        const int otherAssignment = 2;
        var changedValues = new List<ChangedValue>
        {
            GetIndirectAssignment((int)MsgTemplateCode.OtherAssignmentInsert, otherAssignment, 8),
            GetIndirectAssignment((int)MsgTemplateCode.OtherAssignmentDelete, otherAssignment, 9),

            GetWatcherAssignment((int)MsgTemplateCode.OtherAssignmentInsert, otherAssignment, 13),
            GetWatcherAssignment((int)MsgTemplateCode.OtherAssignmentDelete, otherAssignment, 14)
        };

        return changedValues;
    }

    private static ChangedValue GetDirectAssignment(int messageTypeId, int assignmentType, int betTypeId)
    {
        return new ChangedValue
        {
            MessageTypeId = messageTypeId,
            BetTypeId = betTypeId,
            AssignmentType = assignmentType,
            MatchId = 10,
            AssigneeId = 100,
            IsIndirectChangedAssignment = false,
            IsWatcher = false
        };
    }

    private static ChangedValue GetIndirectAssignment(int messageTypeId, int assignmentType, int betTypeId)
    {
        return new ChangedValue
        {
            MessageTypeId = messageTypeId,
            BetTypeId = betTypeId,
            AssignmentType = assignmentType,
            MatchId = 20,
            AssigneeId = 200,
            IsIndirectChangedAssignment = true,
            IsWatcher = false,
            OldValue = messageTypeId == (int)MsgTemplateCode.AssignmentInsert ? string.Empty : "OldValue",
            NewValue = messageTypeId == (int)MsgTemplateCode.AssignmentDelete ? string.Empty : "NewValue"
        };
    }

    private static ChangedValue GetWatcherAssignment(int messageTypeId, int assignmentType, int betTypeId)
    {
        return new ChangedValue
        {
            MessageTypeId = messageTypeId,
            BetTypeId = betTypeId,
            AssignmentType = assignmentType,
            MatchId = 30,
            AssigneeId = 300,
            IsIndirectChangedAssignment = false,
            IsWatcher = true,
            OldValue = messageTypeId == (int)MsgTemplateCode.AssignmentInsert ? string.Empty : "OldValue",
            NewValue = messageTypeId == (int)MsgTemplateCode.AssignmentDelete ? string.Empty : "NewValue"
        };
    }

    private static ChangedValue GetChangedInfoValue()
    {
        return new ChangedValue
        {
            MessageTypeId = (int)MsgTemplateCode.EditWB,
            MatchId = 40,
            AssigneeId = 400,
            IsIndirectChangedAssignment = false
        };
    }

    private static List<ChangedValue> a()
    {
        return new List<ChangedValue>
        {
            new()
            {
                AssigneeId = 15477090 ,
                AssigneeName = "starix15" ,
                AssignmentNote  = "",
                AssignmentType  =1   ,
                BetTypeId = 1  ,
                BetTypeName = "HDP" ,
                EventName = "Esther -vs- Mira" ,
                IsIndirectChangedAssignment   =   false   ,
                IsWatcher = false   ,
                League = "MiraTest" ,
                MatchId = 555594  ,
                MatchNo = "1111111111"  ,
                MessageTypeId =   1   ,
                NewValue = "#starix15" ,
                NotificationSettingId   =   1   ,
                OldValue = null
            },
            new()
            {
                AssigneeId = 15477090 ,
                AssigneeName = "starix15" ,
                AssignmentNote  =  ""  ,
                AssignmentType  =   1   ,
                BetTypeId = 7  ,
                BetTypeName = "1H HDP"  ,
                EventName = "Esther -vs- Mira" ,
                IsIndirectChangedAssignment   =   false   ,
                IsWatcher = false   ,
                League = "MiraTest" ,
                MatchId = 555594  ,
                MatchNo = "1111111111"  ,
                MessageTypeId =   1   ,
                NewValue = "#starix15" ,
                NotificationSettingId   =   1   ,
                OldValue = null
            },
            new()
            {
                AssigneeId = 169368  ,
                AssigneeName = "starix3" ,
                AssignmentNote  =    "",
                AssignmentType  =   1   ,
                BetTypeId = 8  ,
                BetTypeName = "1H O/U"  ,
                EventName = "Esther -vs- Mira" ,
                IsIndirectChangedAssignment   =   false   ,
                IsWatcher = false   ,
                League = "MiraTest" ,
                MatchId = 555594  ,
                MatchNo = "1111111111"  ,
                MessageTypeId =   1   ,
                NewValue = "#starix3"  ,
                NotificationSettingId   =   1   ,
                OldValue = null
            }
        };
    }
}