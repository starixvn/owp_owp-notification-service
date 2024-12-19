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

    Task<Dictionary<string, List<Assignee>>> GetSessionWatcherSettings();
}

public class NotificationSettingQueryRepositoryImp(IDynamicRepository repository) : NotificationSettingQueryRepository
{
    public async Task<HashSet<string>> GetUnsubscribeSettings()
    {
        var criteria = new GetNotificationSettingCommand();
        var notificationSettings = await repository.FetchAsync<NotificationSetting>(criteria);

        return [..notificationSettings.Select(a => $"{a.UserId}-{a.NotificationSettingId}")];
    }

    public async Task<Dictionary<string, List<Assignee>>> GetSessionWatcherSettings()
    {
        var criteria = new GetSessionReceivedWatcherCommand();

        var watchers = (await repository.FetchAsync<SessionWatcher>(criteria)).ToList();

        return watchers.GroupBy(a => $"{a.UserId}-{a.NotificationSettingId}")
            .ToDictionary(key => key.Key, value => value.Select(i => new Assignee
            {
                AssigneeId = i.UserWatcherId,
                AssigneeName = i.UserWatcherName
            }).ToList());
    }
}