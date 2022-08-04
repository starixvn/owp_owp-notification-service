

namespace OWP.OtherSportNotification.UnitTests.Services;

public class AssigneeChangedValueFilterUnitTest
{

    [Fact]
    public void Filter_AssigneeUnSubcribeForMatchInfoUpdatedId_ReturnAssigneeChangedValues()
    {
        var changedValueList = GetListValueChanged();
        var unsubcribeSettings = new HashSet<string> { "6789-5" };
        var assigneeMessageFilter = MockAssigneeChangedValueFilter(unsubcribeSettings);

        var expected = new List<ChangedValue>
        {
            new ChangedValue
            {
                MessageTypeId = (int)MsgTemplateCode.EditOtherProvider,
                NewValue = "Rossi",
                OldValue = "Bow",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                UserId = 6789,
                Username = "esther",
                NotificationSettingId = 11
            }
        };

        var actual = assigneeMessageFilter.Filter(changedValueList);

        DeepAssert.Equal(expected, actual.Result);
    }

    [Fact]
    public void Filter_AssigneeUnSubcribeForProviderInfoUpdatedId_ReturnAssigneeChangedValues()
    {
        var changedValueList = GetListValueChanged();
        var unsubcribeSettings = new HashSet<string> { "6789-11" };
        var assigneeMessageFilter = MockAssigneeChangedValueFilter(unsubcribeSettings);

        var expected = new List<ChangedValue>
        {
            new ChangedValue
            {
                MessageTypeId = (int)MsgTemplateCode.EditMatchNo,
                NewValue = "20000",
                OldValue = "1234",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                UserId = 6789,
                Username = "esther",
                NotificationSettingId = 5
            }
        };

        var actual = assigneeMessageFilter.Filter(changedValueList);

        DeepAssert.Equal(expected, actual.Result);
    }

    private static AssigneeChangedValueFilter MockAssigneeChangedValueFilter(HashSet<string> unsubcribeSettings)
    {
        var repository = Substitute.For<OtherSportRepository>();
        repository.GetUnsubscribeSettings().Returns(unsubcribeSettings);

        return new AssigneeMessageFilterImp(repository);
    }

    private static List<ChangedValue> GetListValueChanged()
    => new List<ChangedValue>
        {
            new ChangedValue
            {
                MessageTypeId = (int)MsgTemplateCode.EditMatchNo,
                NewValue = "20000",
                OldValue = "1234",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                UserId = 6789,
                Username = "esther"
            },
            new ChangedValue
            {
                MessageTypeId = (int)MsgTemplateCode.EditOtherProvider,
                NewValue = "Rossi",
                OldValue = "Bow",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                UserId = 6789,
                Username = "esther"
            }
        };

}
