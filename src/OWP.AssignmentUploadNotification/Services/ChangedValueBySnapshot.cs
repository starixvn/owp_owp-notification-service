namespace OWP.AssignmentUploadNotification.Services;

using Models;
using System.Collections.Generic;
using System.Linq;

public interface ChangedValueBySnapshot
{
    List<ChangedValue> Create(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot);
}

public class ChangedValueBySnapshotImp(
    ChangedDetector changedDetector,
    InsertedMatchChangedValueGenerator insertedMatchChangedValueGenerator,
    DeletedMatchChangedValueGenerator deletedMatchChangedValueGenerator,
    ChangedMatchChangedValueGenerator changedMatchChangedValueGenerator)
    : ChangedValueBySnapshot
{
    public List<ChangedValue> Create(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot)
    {
        var changedSnapshot = changedDetector.GetChangedSnapshot(oldSnapshot, newSnapshot);

        var insertedMatchChangedValues = insertedMatchChangedValueGenerator.Create(changedSnapshot.InsertedMatches, newSnapshot);
        var deletedMatchChangedValues = deletedMatchChangedValueGenerator.Create(changedSnapshot.DeletedMatches, oldSnapshot);
        var changedMatchChangedValues = changedMatchChangedValueGenerator.Create(changedSnapshot.ChangedMatches, oldSnapshot, newSnapshot);

        return new[]
            {
                insertedMatchChangedValues,
                deletedMatchChangedValues,
                changedMatchChangedValues
            }
            .SelectMany(v => v)
            .ToList();
    }
}