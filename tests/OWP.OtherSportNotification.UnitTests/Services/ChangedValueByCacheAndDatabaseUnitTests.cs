namespace OWP.OtherSportNotification.UnitTests.Services;
public class ChangedValueByCacheAndDatabaseUnitTests
{
    private readonly ChangedValueByCacheAndDatabaseImp services;
    private readonly CacheManipulator cacheManipulator;
    private readonly OtherSportRepository repository;
    private readonly ChangedValueBySnapshot changedValueBySnapshot;

    public ChangedValueByCacheAndDatabaseUnitTests()
    {
        cacheManipulator = Substitute.For<CacheManipulator>();
        repository = Substitute.For<OtherSportRepository>();
        changedValueBySnapshot = Substitute.For<ChangedValueBySnapshot>();
        services = new ChangedValueByCacheAndDatabaseImp(cacheManipulator, repository, changedValueBySnapshot);
    }

    [Fact]
    public void Create_Always_ReturnChangedValue()
    {
        // Arrange
        var changedValues = new List<ChangedValue>
            {
                new()
                {
                    UserId = 10
                }
            };
        cacheManipulator.GetAssignmentSnapshot(Arg.Compat.Any<string>()).Returns(new AssignmentSnapshot());
        repository.Query(Arg.Compat.Any<DateTime>(), Arg.Compat.Any<DateTime>()).Returns(new AssignmentSnapshot());
        cacheManipulator.DeleteAssignmentSnapshot(Arg.Compat.Any<string>());
        changedValueBySnapshot.Create(Arg.Compat.Any<AssignmentSnapshot>(), Arg.Compat.Any<AssignmentSnapshot>()).Returns(changedValues);

        // Act
        var actual = services.Create("abc", new DateTime(2022, 06, 10), new DateTime(2022, 06, 11));

        // Assert
        Equals(changedValues, actual);
    }
}
