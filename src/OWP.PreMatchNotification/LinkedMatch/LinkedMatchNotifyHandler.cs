using MediatR;
using OWP.Core.NotificationRequests.Prematch;
using OWP.PreMatchNotification.LinkedMatch.Models;
using OWP.PreMatchNotification.LinkedMatch.Services;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.LinkedMatch;

public class LinkedMatchNotifyHandler : IRequestHandler<LinkedMatchNotifyRequest, Unit>
{
    private readonly CacheManipulator cacheManipulator;
    private readonly Repository queryRepository;
    private readonly ChangedLinkedMatchDetector changedValueBySnapshot;
    private readonly MessageFactory messageFactory;
    private readonly CommandRepository commandRepository;
    private readonly NotifyServices notify;


    public LinkedMatchNotifyHandler(
        CacheManipulator cacheManipulator,
        Repository queryRepository,
        ChangedLinkedMatchDetector changedValueBySnapshot,
        MessageFactory messageFactory,
        CommandRepository commandRepository,
        NotifyServices notify)
    {
        this.cacheManipulator = cacheManipulator;
        this.queryRepository = queryRepository;
        this.changedValueBySnapshot = changedValueBySnapshot;
        this.messageFactory = messageFactory;
        this.commandRepository = commandRepository;
        this.notify = notify;
    }

    public async Task<Unit> Handle(LinkedMatchNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await GenerateMessages(request);
        var messageModels = messageFactory.Transform(messages);

        await commandRepository.InsertMessage(messageModels, request.SessionId);
        await notify.Notify(messageModels);

        await cacheManipulator.DeleteAssignmentSnapshot(request.SessionId);

        return await Task.FromResult(Unit.Value);
    }

    public async Task<List<ChangedValue>> GenerateMessages(LinkedMatchNotifyRequest request)
    {
        var oldMatches = await cacheManipulator.GetOldLGAMatchSnapshot(request.SessionId);

        if (oldMatches.Count > 0)
        {
            var newMatches = await queryRepository.GetLGAMatchNewSnapshot(request.EventDate);

            var employees = await GetEmployees(request.EventDate);

            var messages = changedValueBySnapshot.Create(oldMatches, newMatches, employees);

            return messages;
        }

        return new List<ChangedValue>();
    }

    public async Task<Employee> GetEmployees(DateTime eventDate)
    {
        var prematchLeagues = await queryRepository.GetInfositePrematch(eventDate);
        var allPrematchEmployees = await commandRepository.GetEmployeesBySection(Section.PreMatch);
        var allLSTEmployees = await commandRepository.GetEmployeesBySection(Section.LST);

        var prematchAssignees = await GetPrematchAssigneeIds(prematchLeagues);
        MapAssignmentWithInfositeId(prematchLeagues, prematchAssignees);

        return new Employee
        {
            PrematchInfosites = prematchLeagues.GroupBy(x => x.InfoSiteMatchId).ToDictionary(x => x.Key, x => x.ToList()),
            AllPrematchEmployees = allPrematchEmployees.Select(x => x.AssigneeId).ToList(),
            AllLSTEmployees = allLSTEmployees.Select(x => x.AssigneeId).ToList()
        };
    }

    public void MapAssignmentWithInfositeId(List<InfositePrematchLeague> prematchLeagues, List<Assignee> prematchAssignees)
    {
        foreach (var prematchLeague in prematchLeagues)
        {
            var assignments = prematchAssignees.Where(x => x.PreMatchLeagueId == prematchLeague.PreMatchLeagueId);
            prematchLeague.PrematchAssigneeIds.AddRange(assignments.Select(x => x.AssigneeId));
        }
    }

    public async Task<List<Assignee>> GetPrematchAssigneeIds(List<InfositePrematchLeague> prematchLeagues)
    {
        var prematchLeagueIds = prematchLeagues.Select(x => x.PreMatchLeagueId).Distinct().ToList();
        var prematchAssignees = await commandRepository.GetPreMatchLeagueAssignees(prematchLeagueIds);
        return prematchAssignees.ToList();
    }
}