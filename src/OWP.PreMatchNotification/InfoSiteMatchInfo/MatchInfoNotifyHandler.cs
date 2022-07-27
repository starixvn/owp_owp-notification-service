using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.Prematch;
using OWP.PreMatchNotification.InfoSiteMatchInfo.Models;
using OWP.PreMatchNotification.InfoSiteMatchInfo.Services;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.InfoSiteMatchInfo;

public class MatchInfoNotifyHandler : IRequestHandler<MatchInfoNotifyRequest, string>
{
    private readonly CacheManipulatorCommon cacheManipulator;

    private readonly Repository queryRepository;

    private readonly ChangedValueGenerator changedValueGenerator;

    private readonly MessageFactory messageFactory;

    private readonly CommandRepository commandRepository;

    private const string OLD_INFOSITEMATCHES_SNAPSHOT_KEY = "OWP-Old-InfoSiteMatches";

    public MatchInfoNotifyHandler(
        CacheManipulatorCommon cacheManipulator,
        Repository queryRepository,
        ChangedValueGenerator changedValueGenerator,
        MessageFactory messageFactory,
        CommandRepository commandRepository)
    {
        this.cacheManipulator = cacheManipulator;
        this.queryRepository = queryRepository;
        this.changedValueGenerator = changedValueGenerator;
        this.messageFactory = messageFactory;
        this.commandRepository = commandRepository;
    }

    public async Task<string> Handle(MatchInfoNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await GenerateMessages(request);
        var messageModels = messageFactory.Transform(messages);

        await commandRepository.InsertMessage(messageModels, request.SessionId);

        return JsonConvert.SerializeObject(messageModels);
    }

    public async Task<IList<ChangedValue>> GenerateMessages(MatchInfoNotifyRequest request)
    {
        var snapShotKey = $"{OLD_INFOSITEMATCHES_SNAPSHOT_KEY}-{request.SessionId}";
        var oldSnapshot = await cacheManipulator.GetMultipleData<InfoSiteMatch>(snapShotKey);
        var newSnapshot = await queryRepository.GetInfoSiteMatchNewSnapshot(request.FromDate, request.ToDate);

        await cacheManipulator.RemoveData(snapShotKey);

        var owpLeagueIds = newSnapshot.Select(x => x.OWPLeagueId).ToList();
        var preMatchAssignees = await commandRepository.GetPreMatchLeagueAssigneesByOWPLeagueIds(owpLeagueIds, request.FromDate);
        var lst = await commandRepository.GetEmployeesBySection(Section.LST);

        var assigneeMessages = changedValueGenerator.CreateAssigneeMessages(oldSnapshot, newSnapshot, preMatchAssignees);
        var infoSiteMatchIds = assigneeMessages.Select(x => x.InfoSiteMatchId).Distinct().ToList();
        var linkedMatches = await queryRepository.GetLGALinkedInfoSiteMatch(infoSiteMatchIds);
        var lstMessages = changedValueGenerator.CreateLSTMessages(assigneeMessages, lst, linkedMatches);

        return assigneeMessages.Union(lstMessages).ToList();
    }
}