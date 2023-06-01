using NSubstitute;
using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

public class AssigneeChangedValueFilterUnitTest
{
    private readonly NotificationSettingQueryRepository repository;
    private readonly AssigneeChangedValueFilter assigneeMessageFilter;
    public AssigneeChangedValueFilterUnitTest()
    {
        repository = Substitute.For<NotificationSettingQueryRepository>();

        assigneeMessageFilter = new AssigneeMessageFilterImp(repository);
    }

    [Fact]
    public void Filter_AssigneeUnSubcribeForMatchInfoUpdatedId_ReturnAssigneeChangedValues()
    {
        var changedValueList = GetListValueChanged();
        var unsubcribeSettings = new HashSet<string> { "6789-5" };

        var expected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 25,
                NewValue = "Rossi",
                OldValue = "Bow",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther",
                NotificationSettingId = 11
            }
        };

        repository.GetUnsubscribeSettings().Returns(unsubcribeSettings);
        var actual = assigneeMessageFilter.Filter(changedValueList);

        DeepAssert.Equal(expected, actual.Result);
    }

    [Fact]
    public void Filter_AssigneeUnSubcribeForProviderInfoUpdatedId_ReturnAssigneeChangedValues()
    {
        var changedValueList = GetListValueChanged();
        var unsubcribeSettings = new HashSet<string> { "6789-11" };

        var expected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 40,
                NewValue = "20000",
                OldValue = "1234",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther",
                NotificationSettingId = 5
            }
        };

        repository.GetUnsubscribeSettings().Returns(unsubcribeSettings);
        var actual = assigneeMessageFilter.Filter(changedValueList);

        DeepAssert.Equal(expected, actual.Result);
    }

    private List<ChangedValue> GetListValueChanged()
    {
        var valueChanged = new ChangedValue
        {
            MessageTypeId = 40,
            NewValue = "20000",
            OldValue = "1234",
            League = "PremierLeague-Coners",
            MatchNo = "20000",
            EventName = "LiverPool-Coners -vs- Arsenal-Coners",
            AssigneeId = 6789,
            AssigneeName = "esther"
        };

        var valueChanged2 = new ChangedValue
        {
            MessageTypeId = 25,
            NewValue = "Rossi",
            OldValue = "Bow",
            League = "PremierLeague-Coners",
            MatchNo = "20000",
            EventName = "LiverPool-Coners -vs- Arsenal-Coners",
            AssigneeId = 6789,
            AssigneeName = "esther"
        };

        var changedValueList = new List<ChangedValue>
        {
            valueChanged,
            valueChanged2
        };

        return changedValueList;
    }
}