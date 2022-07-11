using Fanex.Data.Repository;
using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.Prematch;
using OWP.PreMatchNotification.Criteria;
using OWP.PreMatchNotification.NewLeague.Criteria;
using OWP.PreMatchNotification.NewLeague.Models;
using OWP.PreMatchNotification.NewLeague.Services;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.NewLeague;

public class NewLeagueNotifyHandler : IRequestHandler<PrematchNewLeagueNotifyRequest, Unit>
{
    private readonly CacheManipulatorCommon cacheManipulator;

    private readonly IDynamicRepository dynamicRepository;

    private readonly NewLeagueValueGenerator newLeagueGenerator;

    private readonly MessageFactory messageFactory;

    private readonly CommandRepository commandRepository;

    private readonly NotifyServices notify;

    private const string OLD_PREMATCHLEAGUES_SNAPSHOT_KEY = "OWP-Old-PreMatchLeagues";

    public NewLeagueNotifyHandler(
        CacheManipulatorCommon cacheManipulator,
        IDynamicRepository dynamicRepository,
        NewLeagueValueGenerator newLeagueGenerator,
        MessageFactory messageFactory,
        CommandRepository commandRepository,
        NotifyServices notify)
    {
        this.cacheManipulator = cacheManipulator;
        this.dynamicRepository = dynamicRepository;
        this.newLeagueGenerator = newLeagueGenerator;
        this.messageFactory = messageFactory;
        this.commandRepository = commandRepository;
        this.notify = notify;
    }

    public async Task<Unit> Handle(PrematchNewLeagueNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await GenerateMessages(request);
        var messageModels = messageFactory.Transform(messages);

        await commandRepository.InsertMessage(messageModels, request.SessionId);

        await notify.Notify(messageModels);

        var mesage = JsonConvert.SerializeObject(messageModels);

        return await Task.FromResult(Unit.Value);
    }

    private async Task<IList<ChangedValue>> GenerateMessages(PrematchNewLeagueNotifyRequest request)
    {
        var oldSnapshot = await cacheManipulator.GetMultipleData<PreMatchLeague>($"{OLD_PREMATCHLEAGUES_SNAPSHOT_KEY}-{request.SessionId}");
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

    private async Task<IList<InfoSiteMatch>> GetInfoSiteMatch(List<int> owpLeagueIds, DateTime fromDate)
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