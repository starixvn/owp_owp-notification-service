namespace OWP.AssignmentUploadNotification.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Models;

public interface AssigneeChangedValueFilter
{
    Task<List<ChangedValue>> Filter(List<ChangedValue> changedValues);
}

public class AssigneeMessageFilterImp(NotificationSettingQueryRepository repository) : AssigneeChangedValueFilter
{
    public async Task<List<ChangedValue>> Filter(List<ChangedValue> changedValues)
    {
        changedValues.ForEach(c => c.NotificationSettingId = NotificationSettingHelper.GetNotificationSettingIdForAssignee(c.MessageTypeId, c.BetTypeId, c.IsIndirectChangedAssignment));

        var unsubscribeSettings = await repository.GetUnsubscribeSettings();
        return changedValues.Where(m => !unsubscribeSettings.Contains($"{m.AssigneeId}-{m.NotificationSettingId}")).ToList();
    }
}