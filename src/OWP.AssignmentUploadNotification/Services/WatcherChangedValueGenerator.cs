
namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Extensions;
    using Models;
    using OWP.Core.Extensions;

    public interface WatcherChangedValueGenerator
    {
        Task<List<ChangedValue>> Create(List<ChangedValue> changedValues, List<ChangedValue> assigneeChangedValues);
    }

    public class WatcherChangedValueGeneratorImp : WatcherChangedValueGenerator
    {
        private readonly NotificationSettingQueryRepository repository;
        private readonly IMapper mapper;

        public WatcherChangedValueGeneratorImp(NotificationSettingQueryRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<ChangedValue>> Create(List<ChangedValue> changedValues, List<ChangedValue> assigneeChangedValues)
        {
            var settings = await repository.GetSessionWatcherSettings();

            var assigneesOfMatches = GetAssignees(assigneeChangedValues);

            var watcherChangedValues = Create(changedValues, settings, assigneesOfMatches);

            return watcherChangedValues.SelectMany(m => m).ToList();
        }

        public IEnumerable<IEnumerable<ChangedValue>> Create(List<ChangedValue> changedValues, Dictionary<string, List<Assignee>> settings, Dictionary<int, List<Assignee>> assigneesOfMatches)
        {
            foreach (var changedValue in changedValues)
            {
                var notificationSettingId = NotificationSettingHelper.GetNotificationSettingIdForWatcher(changedValue.MessageTypeId, changedValue.BetTypeId);

                var watchers = settings.GetValue($"{changedValue.AssigneeId}-{notificationSettingId}", new List<Assignee>());
                var assignees = assigneesOfMatches.GetValue(changedValue.MatchId, new List<Assignee>());
                var watchersIgnoredAssignees = watchers.Where(x => x.AssigneeId != changedValue.AssigneeId)
                                                       .Except(assignees, new Assignee())
                                                       .Distinct(new Assignee())
                                                       .ToList();

                yield return watchersIgnoredAssignees.Select(w =>
                {
                    var watcherChangedValue = mapper.Map<ChangedValue>(changedValue);
                    watcherChangedValue.AssigneeId = w.AssigneeId;
                    watcherChangedValue.AssigneeName = w.AssigneeName;
                    watcherChangedValue.IsWatcher = true;
                    watcherChangedValue.NotificationSettingId = notificationSettingId;

                    return watcherChangedValue;
                });
            }
        }

        public static Dictionary<int, List<Assignee>> GetAssignees(List<ChangedValue> changedValues)
        {
            return changedValues.Where(x => !NotificationSettingHelper.IsUpdateAssignment(x.MessageTypeId) ||
                                       x.IsIndirectChangedAssignment)
                                .GroupBy(x => x.MatchId)
                                .ToDictionary(x => x.Key, x => x.Select(a => new Assignee
                                {
                                    AssigneeId = a.AssigneeId,
                                    AssigneeName = a.AssigneeName
                                }).Distinct(new Assignee()).ToList());
        }
    }
}
