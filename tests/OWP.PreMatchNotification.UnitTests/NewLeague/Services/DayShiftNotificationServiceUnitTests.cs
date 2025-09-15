using OWP.PreMatchNotification.NewLeague.Criteria;
using OWP.PreMatchNotification.NewLeague.Services;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.UnitTests.NewLeague.Services;

public class DayShiftNotificationServiceUnitTests
{
    private readonly CommandRepository _mockRepository;
    private readonly NotificationServiceCommon _mockNotificationServiceCommon;
    private readonly DayShiftNotificationService _service;

    public DayShiftNotificationServiceUnitTests()
    {
        _mockRepository = Substitute.For<CommandRepository>();
        _mockNotificationServiceCommon = Substitute.For<NotificationServiceCommon>();
        _service = new DayShiftNotificationServiceImp(_mockNotificationServiceCommon, _mockRepository);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var expectedLeagues = new List<string> { "Premier League", "La Liga", "Bundesliga" };
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(expectedLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryDayShiftPreMatchAssignments(Arg.Is<QueryShiftPreMatchCriteria>(criteria =>
            criteria != null));
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldCallNotificationServiceCommonWithCorrectParameters()
    {
        // Arrange
        var expectedLeagues = new List<string> { "Premier League", "La Liga" };
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(expectedLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            expectedLeagues,
            expectedDate,
            true);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleEmptyLeagueList()
    {
        // Arrange
        var emptyLeagues = new List<string>();
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(emptyLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            emptyLeagues,
            Arg.Any<DateTime>(),
            true);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleRepositoryException()
    {
        // Arrange
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(Task.FromException<List<string>>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SendNotificationForNewLeague());
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleNotificationServiceException()
    {
        // Arrange
        var expectedLeagues = new List<string> { "Premier League" };
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(expectedLeagues);
        _mockNotificationServiceCommon.SendNotificationForNewLeague(Arg.Any<IList<string>>(), Arg.Any<DateTime>(), Arg.Any<bool>())
            .Returns(Task.FromException(new Exception("Notification service error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SendNotificationForNewLeague());
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldUseCorrectDateCalculation()
    {
        // Arrange
        var expectedLeagues = new List<string> { "Test League" };
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(expectedLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            expectedLeagues,
            Arg.Is<DateTime>(date => date == DateTimeOffset.Now.AddHours(12).Date),
            true);
    }
}
