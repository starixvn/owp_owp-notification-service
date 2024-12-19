namespace OWP.AssignmentUploadNotification.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

public interface ChangedValueByCacheAndDatabase
{
    Task<List<ChangedValue>> Create(string sessionId, DateTime eventDate);
}

public class ChangedValueCacheAndDatabaseImp(
    CacheManipulator cacheManipulator,
    QueryRepository queryRepository,
    ChangedValueBySnapshot changedValueBySnapshot)
    : ChangedValueByCacheAndDatabase
{
    public async Task<List<ChangedValue>> Create(string sessionId, DateTime eventDate)
    {
        var oldSnapshot = await cacheManipulator.GetAssignmentSnapshot(sessionId);
        var newSnapshot = await queryRepository.Query(eventDate);

        return changedValueBySnapshot.Create(oldSnapshot, newSnapshot);
    }
}