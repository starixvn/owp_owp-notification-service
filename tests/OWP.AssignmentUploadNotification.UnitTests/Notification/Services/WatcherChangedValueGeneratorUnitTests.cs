using AutoMapper;
using NSubstitute;
using OWP.AssignmentUploadNotification.Constants;
using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using Xunit;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

public class WatcherChangedValueGeneratorUnitTests
{
    private readonly WatcherChangedValueGeneratorImp services;
    private readonly NotificationSettingQueryRepository repository;

    public WatcherChangedValueGeneratorUnitTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ChangedValue, ChangedValue>();
        });
        var mapper = configuration.CreateMapper();

        repository = Substitute.For<NotificationSettingQueryRepository>();

        services = new WatcherChangedValueGeneratorImp(repository, mapper);
    }

    [Fact]
    public async Task Create()
    {
        // Arrange
        var changedValue = GetChangeValue();
        var listWatcherChangeValue = GetWatchersChangeValue();

        repository.GetSessionWatcherSettings().Returns(listWatcherChangeValue);

        // Act
        var actual = await services.Create(changedValue, new List<ChangedValue>());

        // Assert
        Assert.Equal(4, actual.Count);
        Assert.Equal(1, actual.Count(item => item.AssigneeId == 8 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert));
        Assert.Equal(1, actual.Count(item => item.AssigneeId == 15477084 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete));
        Assert.Equal(1, actual.Count(item => item.AssigneeId == 8862400 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete));
        Assert.Equal(1, actual.Count(item => item.AssigneeId == 360418 & item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete));
    }

    [Fact]
    public void GetAssignees()
    {
        // Arrange
        var changedValues = new List<ChangedValue>
        {
            GetDirectChangeValue(1, 15477090, 1, (int)MsgTemplateCode.ChangeOfAssignment),
            GetIndirectChangeValue(1, 169368, 1, (int)MsgTemplateCode.AssignmentDelete)
        };

        // Act
        var actual = WatcherChangedValueGeneratorImp.GetAssignees(changedValues);

        // Assert
        Assert.Single(actual);
    }

    public List<ChangedValue> GetChangeValue()
    {
        return new List<ChangedValue>
        {
            GetDirectChangeValue(1, 15477090, 1, (int)MsgTemplateCode.AssignmentInsert),
            GetIndirectChangeValue(1, 169368, 1, (int)MsgTemplateCode.AssignmentDelete)
        };
    }

    public ChangedValue GetDirectChangeValue(int matchId, int assigneeId, int betTypeId, int messageTypeId)
    {
        return new ChangedValue
        {
            MatchId = matchId,
            AssigneeId = assigneeId,
            BetTypeId = betTypeId,
            MessageTypeId = messageTypeId,
            OldValue = messageTypeId == (int)MsgTemplateCode.AssignmentInsert ? null : "OldValue",
            NewValue = messageTypeId == (int)MsgTemplateCode.AssignmentDelete ? null : "NewValue",
            IsIndirectChangedAssignment = false
        };
    }

    public ChangedValue GetIndirectChangeValue(int matchId, int assigneeId, int betTypeId, int messageTypeId)
    {
        return new ChangedValue
        {
            MatchId = matchId,
            AssigneeId = assigneeId,
            BetTypeId = betTypeId,
            MessageTypeId = messageTypeId,
            OldValue = messageTypeId == (int)MsgTemplateCode.AssignmentInsert ? null : "OldValue",
            NewValue = messageTypeId == (int)MsgTemplateCode.AssignmentDelete ? null : "NewValue",
            IsIndirectChangedAssignment = true
        };
    }

    public Dictionary<string, List<Assignee>> GetWatchersChangeValue()
    {
        return new Dictionary<string, List<Assignee>>
        {
            {
                "15477090-6",
                new List<Assignee> { new() { AssigneeId = 8, AssigneeName = "sa" } }
            },
            {
                "15477090-13",
                new List<Assignee> { new() { AssigneeId = 8, AssigneeName = "sa" } }
            },
            {
                "169368-6",
                new List<Assignee> {
                    new() { AssigneeId = 15477084, AssigneeName = "starix14" },
                    new() { AssigneeId = 8862400, AssigneeName = "starix2" },
                    new() { AssigneeId = 360418, AssigneeName = "starix1" },
                    new() { AssigneeId = 169368, AssigneeName = "starix3" }
                }
            }
        };
    }
}