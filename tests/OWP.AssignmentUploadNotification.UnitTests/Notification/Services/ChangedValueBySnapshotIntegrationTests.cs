using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using OWP.AssignmentUploadNotification.UnitTests.TestUtility;
using Xunit;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

public class ChangedValueBySnapshotIntegrationTests
{
    [Fact]
    public void Create_1DeleteMatch_1EditMatch_1InsertMatch_ReturnChangedValueList()
    {
        // Arrange
        var oldSnapshot = TestDataLoader.Load<AssignmentSnapshot>("ChangedValueBySnapshotImp-1match-OldSnapshot.json");
        var newSnapshot = TestDataLoader.Load<AssignmentSnapshot>("ChangedValueBySnapshotImp-1deleted-1new-1edit-newSnapshot.json");

        var service = CreateChangedValueBySnapshot();

        // Act
        var actual = service.Create(oldSnapshot, newSnapshot);

        // Assert
        Assert.NotEmpty(actual);
    }

    private static ChangedValueBySnapshotImp CreateChangedValueBySnapshot()
    {
        var changeDetector = new ChangedDetectorImp();
        var insertedMatchChangedValueGenerator = new InsertedMatchChangedValueGeneratorImp();
        var deletedMatchChangedValueGenerator = new DeletedMatchChangedValueGeneratorImp();

        var changedMatchInfoGenerator = new ChangedMatchInfoGeneratorImp();
        var changedAssignmentGenerator = new ChangedAssignmentGeneratorImp(changeDetector);
        var changedMatchSettingGenerator = new ChangedMatchSettingGeneratorImp();
        var changedMatchStatusGenerator = new ChangedMatchStatusGeneratorImp();
        var changedMatchChangedValueGenerator = new ChangedMatchChangedValueGeneratorImp(
            changedMatchInfoGenerator,
            changedAssignmentGenerator,
            changedMatchSettingGenerator,
            changedMatchStatusGenerator);

        return new ChangedValueBySnapshotImp(
            changeDetector,
            insertedMatchChangedValueGenerator,
            deletedMatchChangedValueGenerator,
            changedMatchChangedValueGenerator);
    }
}