namespace OWP.AssignmentUploadNotification.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Criteria;
using Fanex.Data.Repository;
using Models;

public interface NotificationSettingQueryRepository
{
    Task<HashSet<string>> GetUnsubscribeSettings();
}

public class NotificationSettingQueryRepositoryImp(IDynamicRepository repository) : NotificationSettingQueryRepository
{
    public async Task<HashSet<string>> GetUnsubscribeSettings()
    {
        var criteria = new GetNotificationSettingCommand();
        var notificationSettings = await repository.FetchAsync<NotificationSetting>(criteria);

        return [..notificationSettings.Select(a => $"{a.UserId}-{a.NotificationSettingId}")];
    }
}