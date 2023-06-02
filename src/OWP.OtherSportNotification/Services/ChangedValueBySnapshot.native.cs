using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface ChangedValueBySnapshot
{
    List<ChangedValue> Create(AssignmentSnapshot oldSnapshot, AssignmentSnapshot newSnapshot);
}

public class ChangedValueBySnapshotImp : ChangedValueBySnapshot
{
    private readonly ChangedDetector changedDetector;
    private readonly InsertedMatchChangedValueGenerator insertedMatchChangedValueGenerator;
    private readonly DeletedMatchChangedValueGenerator deletedMatchChangedValueGenerator;
    private readonly ChangedMatchChangedValueGenerator changedMatchChangedValueGenerator;

    public ChangedValueBySnapshotImp(
        ChangedDetector changedDetector,
        InsertedMatchChangedValueGenerator insertedMatchChangedValueGenerator,
        DeletedMatchChangedValueGenerator deletedMatchChangedValueGenerator,
        ChangedMatchChangedValueGenerator changedMatchChangedValueGenerator)
    {
        this.changedDetector = changedDetector;
        this.insertedMatchChangedValueGenerator = insertedMatchChangedValueGenerator;
        this.deletedMatchChangedValueGenerator = deletedMatchChangedValueGenerator;
        this.changedMatchChangedValueGenerator = changedMatchChangedValueGenerator;
    }

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
