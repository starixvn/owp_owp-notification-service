using NSubstitute;
using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using Xunit;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

public class ChangedValueByCacheAndDatabaseUnitTests
{
    private readonly ChangedValueCacheAndDatabaseImp services;
    private readonly CacheManipulator cacheManipulator;
    private readonly QueryRepository queryRepository;
    private readonly ChangedValueBySnapshot changedValueBySnapshot;

    public ChangedValueByCacheAndDatabaseUnitTests()
    {
        cacheManipulator = Substitute.For<CacheManipulator>();
        queryRepository = Substitute.For<QueryRepository>();
        changedValueBySnapshot = Substitute.For<ChangedValueBySnapshot>();
        services = new ChangedValueCacheAndDatabaseImp(cacheManipulator, queryRepository, changedValueBySnapshot);
    }

    [Fact]
    public void Create_Always_ReturnChangedValue()
    {
        // Arrange
        var changedValues = new List<ChangedValue>
        {
            new()
            {
                AssigneeId = 10
            }
        };
        cacheManipulator.GetAssignmentSnapshot(Arg.Compat.Any<string>()).Returns(new AssignmentSnapshot());
        queryRepository.Query(Arg.Compat.Any<DateTime>()).Returns(new AssignmentSnapshot());
        changedValueBySnapshot.Create(Arg.Compat.Any<AssignmentSnapshot>(), Arg.Compat.Any<AssignmentSnapshot>()).Returns(changedValues);

        // Act
        var actual = services.Create("abc", new DateTime(2020, 12, 10));

        // Assert
        Equals(changedValues, actual);
    }
}