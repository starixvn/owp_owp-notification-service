

namespace OWP.OtherSportNotification.UnitTests.Services;

public class AssigneeChangedValueFilterUnitTest
{

    [Fact]
    public async Task Filter_AssigneeUnSubscribeForMatchInfoUpdatedId_ReturnAssigneeChangedValues()
    {
        var changedValueList = GetListValueChanged();
        var unsubscribeSettings = new HashSet<string> { "6789-5" };
        var assigneeMessageFilter = MockAssigneeChangedValueFilter(unsubscribeSettings);

        var expected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = (int)MsgTemplateCode.EditOtherProvider,
                NewValue = "Rose",
                OldValue = "Bow",
                League = "PremierLeague-Corners",
                MatchNo = "20000",
                EventName = "LiverPool-Corners -vs- Arsenal-Corners",
                UserId = 6789,
                Username = "esther",
                NotificationSettingId = 11
            }
        };

        var actual = await assigneeMessageFilter.Filter(changedValueList);

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public async Task FilterAssigneeUnSubsFilterAssigneeUnSubscribeForProviderInfoUpdatedIdReturnAssigneeChangedValues()
    {
        var changedValueList = GetListValueChanged();
        var unsubscribeSettings = new HashSet<string> { "6789-11" };
        var assigneeMessageFilter = MockAssigneeChangedValueFilter(unsubscribeSettings);

        var expected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = (int)MsgTemplateCode.EditMatchNo,
                NewValue = "20000",
                OldValue = "1234",
                League = "PremierLeague-Corners",
                MatchNo = "20000",
                EventName = "LiverPool-Corners -vs- Arsenal-Corners",
                UserId = 6789,
                Username = "esther",
                NotificationSettingId = 5
            }
        };

        var actual = await assigneeMessageFilter.Filter(changedValueList);

        DeepAssert.Equal(expected, actual);
    }

    private static AssigneeChangedValueFilter MockAssigneeChangedValueFilter(HashSet<string> unsubscribeSettings)
    {
        var repository = Substitute.For<OtherSportRepository>();
        repository.GetUnsubscribeSettings().Returns(unsubscribeSettings);

        return new AssigneeMessageFilterImp(repository);
    }

    private static List<ChangedValue> GetListValueChanged()
    =>
    [
        new()
        {
            MessageTypeId = (int)MsgTemplateCode.EditMatchNo,
            NewValue = "20000",
            OldValue = "1234",
            League = "PremierLeague-Corners",
            MatchNo = "20000",
            EventName = "LiverPool-Corners -vs- Arsenal-Corners",
            UserId = 6789,
            Username = "esther"
        },

        new()
        {
            MessageTypeId = (int)MsgTemplateCode.EditOtherProvider,
            NewValue = "Rose",
            OldValue = "Bow",
            League = "PremierLeague-Corners",
            MatchNo = "20000",
            EventName = "LiverPool-Corners -vs- Arsenal-Corners",
            UserId = 6789,
            Username = "esther"
        }
    ];

}
