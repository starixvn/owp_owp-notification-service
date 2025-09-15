using Fanex.Data.Repository;
using MediatR;
using OWP.Core.NotificationRequests.Prematch;
using OWP.PreMatchNotification.NewLeague.Criteria;
using OWP.PreMatchNotification.NewLeague.Models;
using OWP.PreMatchNotification.NewLeague.Services;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Criteria;
using OWP.PreMatchNotification.Shared.Models;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.NewLeague;

public class NewLeagueNotifyHandler(
    CacheManipulatorCommon cacheManipulator,
    IDynamicRepository dynamicRepository,
    NewLeagueValueGenerator newLeagueGenerator,
    MessageFactory messageFactory,
    CommandRepository commandRepository,
    NotificationService notificationService,
    DayShiftNotificationService dayShiftNotification,
    NightShiftNotificationService nightShiftNotification)
    : IRequestHandler<PrematchNewLeagueNotifyRequest, Unit>
{
    private const string OldPrematchLeaguesSnapshotKey = "OWP-Old-PreMatchLeagues";

    public async Task<Unit> Handle(PrematchNewLeagueNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await GenerateMessages(request);
        var messageModels = messageFactory.Transform(messages);

        await commandRepository.InsertMessage(messageModels, request.SessionId);

        await notificationService.Notify(messageModels);
        await dayShiftNotification.SendNotificationForNewLeague();
        await nightShiftNotification.SendNotificationForNewLeague();


        return Unit.Value;
    }

    private async Task<IList<ChangedValue>> GenerateMessages(PrematchNewLeagueNotifyRequest request)
    {
        var cacheKey = $"{OldPrematchLeaguesSnapshotKey}-{request.SessionId}";
        var oldSnapshot = await cacheManipulator.GetMultipleData<PreMatchLeague>(cacheKey);
        await cacheManipulator.RemoveData(cacheKey);

        var newSnapshot = await GetPreMatchLeagueNewSnapshot(request.FromDate);

        var preMatchAssignees = await commandRepository.GetEmployeesBySection(Section.PreMatch);
        var infoSiteMatches = await GetInfoSiteMatch(newSnapshot.Select(x => x.OWPLeagueId).ToList(), request.FromDate);

        var messages = newLeagueGenerator.Create(oldSnapshot, newSnapshot, preMatchAssignees, infoSiteMatches);
        return messages.ToList();
    }

    private async Task<IList<PreMatchLeague>> GetPreMatchLeagueNewSnapshot(DateTime fromDate)
    {
        var criteria = new GetPreMatchLeaguesCriteria
        {
            FromDate = fromDate
        };
        var result = await dynamicRepository.FetchAsync<PreMatchLeague>(criteria).ConfigureAwait(false);

        return result.ToList();
    }

    private async Task<IList<InfoSiteMatch>> GetInfoSiteMatch(IEnumerable<int> owpLeagueIds, DateTime fromDate)
    {
        var criteria = new QueryInfoSiteMatchByOWPLeagueIdsCriteria
        {
            OWPLeagueIds = string.Join(',', owpLeagueIds),
            FromDate = fromDate.AddHours(12),
            ToDate = fromDate.AddDays(1).AddHours(11).AddMinutes(59)
        };
        var result = await dynamicRepository.FetchAsync<InfoSiteMatch>(criteria).ConfigureAwait(false);

        return result.ToList();
    }
}