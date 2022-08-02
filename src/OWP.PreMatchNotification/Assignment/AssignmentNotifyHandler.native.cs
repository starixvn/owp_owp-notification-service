using Fanex.Data.Repository;
using MediatR;
using OWP.Core.NotificationRequests.Prematch;
using OWP.PreMatchNotification.Assignment.Criteria;
using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Assignment.Services.MyAssignment;
using OWP.PreMatchNotification.Assignment.Services.OtherAssignment;
using OWP.PreMatchNotification.Shared.Models;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.Assignment;

public class MathInfoNotifyHandler : IRequestHandler<PrematchAssignmentNotifyRequest, Unit>
{
    private readonly CacheManipulatorCommon cacheManipulator;

    private readonly IDynamicRepository dynamicRepository;

    private readonly MyAssignmentGenerator myAssignmentGenerator;

    private readonly OtherAssignmentGenerator otherAssignmentGenerator;

    private readonly MessageFactory messageFactory;

    private readonly CommandRepository commandRepository;

    private readonly NotificationService notificationService;

    public MathInfoNotifyHandler(
        CacheManipulatorCommon cacheManipulator,
        IDynamicRepository dynamicRepository,
        MyAssignmentGenerator myAssignmentGenerator,
        MessageFactory messageFactory,
        CommandRepository commandRepository,
        OtherAssignmentGenerator otherAssignmentGenerator,
        NotificationService notificationService)
    {
        this.cacheManipulator = cacheManipulator;
        this.dynamicRepository = dynamicRepository;
        this.myAssignmentGenerator = myAssignmentGenerator;
        this.messageFactory = messageFactory;
        this.commandRepository = commandRepository;
        this.otherAssignmentGenerator = otherAssignmentGenerator;
        this.notificationService = notificationService;
    }

    public async Task<Unit> Handle(PrematchAssignmentNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await GenerateMessages(request);
        var messageModels = messageFactory.Transform(messages);

        await commandRepository.InsertMessage(messageModels, request.SessionId);

        await notificationService.Notify(messageModels);

        return Unit.Value;
    }

    private async Task<IList<ChangedValue>> GenerateMessages(PrematchAssignmentNotifyRequest request)
    {
        var changedValuesResult = new List<ChangedValue>();
        var oldSnapshot = await cacheManipulator.GetMultipleData<AssignmentSnapshot>(request.RedisKey);
        await cacheManipulator.RemoveData(request.RedisKey);

        var preMatchLeagueIdUpdated = oldSnapshot.Select(item => item.PreMatchLeagueId).ToList();
        var newSnapshot = await GetAssignmentNewSnapshot(preMatchLeagueIdUpdated);
        var prematchIdUpdated = newSnapshot.Select(p => p.PreMatchLeagueId);

        foreach (var prematchId in prematchIdUpdated)
        {
            var oldAssignment = oldSnapshot.First(snap => snap.PreMatchLeagueId == prematchId);
            var newAssignment = newSnapshot.First(snap => snap.PreMatchLeagueId == prematchId);
            var myAssignmentChangedValues = myAssignmentGenerator.Create(oldAssignment, newAssignment);
            var otherAssignmentChangedValues = otherAssignmentGenerator.Create(oldAssignment, newAssignment);

            changedValuesResult.AddRange(myAssignmentChangedValues);
            changedValuesResult.AddRange(otherAssignmentChangedValues);
        }

        return changedValuesResult;
    }

    private async Task<IList<AssignmentSnapshot>> GetAssignmentNewSnapshot(IList<int> prematchIds)
    {
        var assignmentOldValue = new List<AssignmentSnapshot>();
        foreach (var id in prematchIds)
        {
            var assignmentResult = await dynamicRepository.FetchAsync<AssignmentSnapshot>(new QueryAssignmentByPrematchLeagueIdCriteria
            {
                PreMatchLeagueId = id
            });
            assignmentOldValue.Add(assignmentResult.FirstOrDefault());
        }

        return assignmentOldValue;
    }
}