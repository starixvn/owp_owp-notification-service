using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedValueByCacheAndDatabase
{
    Task<List<ChangedValue>> Create(string sessionId, DateTime fromDate, DateTime toDate);
}

public class ChangedValueByCacheAndDatabaseImp(
    CacheManipulator cacheManipulator,
    OtherSportRepository repository,
    ChangedValueBySnapshot changedValueBySnapshot)
    : ChangedValueByCacheAndDatabase
{
    public async Task<List<ChangedValue>> Create(string sessionId, DateTime fromDate, DateTime toDate)
    {
        var oldSnapshot = await cacheManipulator.GetAssignmentSnapshot(sessionId);
        var newSnapshot = await repository.Query(fromDate, toDate);

        await cacheManipulator.DeleteAssignmentSnapshot(sessionId);

        return changedValueBySnapshot.Create(oldSnapshot, newSnapshot);
    }
}
