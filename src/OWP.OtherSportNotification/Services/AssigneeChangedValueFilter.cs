using OWP.OtherSportNotification.Helpers;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface AssigneeChangedValueFilter
{
    Task<List<ChangedValue>> Filter(List<ChangedValue> changedValues);
}

internal class AssigneeMessageFilterImp : AssigneeChangedValueFilter
{
    private readonly OtherSportRepository repository;

    public AssigneeMessageFilterImp(OtherSportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<List<ChangedValue>> Filter(List<ChangedValue> changedValues)
    {
        changedValues.ForEach(c => c.NotificationSettingId = NotificationSettingHelper.GetNotificationSettingIdForAssignee(c.MessageTypeId, c.IsIndirectChangedAssignment));

        var unsubcribeSettings = await repository.GetUnsubscribeSettings();
        return changedValues.Where(m => !unsubcribeSettings.Contains($"{m.UserId}-{m.NotificationSettingId}")).ToList();
    }
}
