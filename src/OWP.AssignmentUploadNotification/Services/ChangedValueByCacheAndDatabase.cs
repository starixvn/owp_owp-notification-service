namespace OWP.AssignmentUploadNotification.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

public interface ChangedValueByCacheAndDatabase
{
    Task<List<ChangedValue>> Create(string sessionId, DateTime eventDate);
}

public class ChangedValueCacheAndDatabaseImp : ChangedValueByCacheAndDatabase
{
    private readonly CacheManipulator cacheManipulator;
    private readonly QueryRepository queryRepository;
    private readonly ChangedValueBySnapshot changedValueBySnapshot;

    public ChangedValueCacheAndDatabaseImp(
        CacheManipulator cacheManipulator,
        QueryRepository queryRepository,
        ChangedValueBySnapshot changedValueBySnapshot)
    {
        this.cacheManipulator = cacheManipulator;
        this.queryRepository = queryRepository;
        this.changedValueBySnapshot = changedValueBySnapshot;
    }

    public async Task<List<ChangedValue>> Create(string sessionId, DateTime eventDate)
    {
        var oldSnapshot = await cacheManipulator.GetAssignmentSnapshot(sessionId);
        var newSnapshot = await queryRepository.Query(eventDate);

        return changedValueBySnapshot.Create(oldSnapshot, newSnapshot);
    }
}