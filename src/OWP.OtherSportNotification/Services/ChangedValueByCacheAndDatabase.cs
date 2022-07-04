using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedValueByCacheAndDatabase
{
    Task<List<ChangedValue>> Create(string sessionId, string eventDate);
}

internal class ChangedValueByCacheAndDatabaseImp : ChangedValueByCacheAndDatabase
{
    private readonly CacheManipulator cacheManipulator;
    private readonly OtherSportRepository repository;
    private readonly ChangedValueBySnapshot changedValueBySnapshot;

    public ChangedValueByCacheAndDatabaseImp(CacheManipulator cacheManipulator, OtherSportRepository repository, ChangedValueBySnapshot changedValueBySnapshot)
    {
        this.cacheManipulator = cacheManipulator;
        this.repository = repository;
        this.changedValueBySnapshot = changedValueBySnapshot;
    }

    public async Task<List<ChangedValue>> Create(string sessionId, string eventDate)
    {
        var oldSnapshot = await cacheManipulator.GetAssignmentSnapshot(sessionId, eventDate);
        var newSnapshot = await repository.Query(eventDate);

        await cacheManipulator.DeleteAssignmentSnapshot(sessionId, eventDate);

        return changedValueBySnapshot.Create(oldSnapshot, newSnapshot);
    }
}
