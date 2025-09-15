using OWP.PreMatchNotification.NewLeague.Criteria;
using OWP.PreMatchNotification.NewLeague.Services;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.UnitTests.NewLeague.Services;

public class NightShiftNotificationServiceUnitTests
{
    private readonly CommandRepository _mockRepository;
    private readonly NotificationServiceCommon _mockNotificationServiceCommon;
    private readonly NightShiftNotificationService _service;

    public NightShiftNotificationServiceUnitTests()
    {
        _mockRepository = Substitute.For<CommandRepository>();
        _mockNotificationServiceCommon = Substitute.For<NotificationServiceCommon>();
        _service = new NightShiftNotificationServiceImp(_mockRepository, _mockNotificationServiceCommon);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_WhenStartTimeExists_ShouldCallRepositoryAndNotificationService()
    {
        // Arrange
        var expectedLeagues = new List<string> { "Premier League", "La Liga", "Bundesliga" };
        var expectedStartTime = TimeSpan.FromHours(18);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(expectedLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryExistedStartTime(expectedDate);
        await _mockRepository.Received(1).QueryDayShiftPreMatchAssignments(Arg.Is<QueryShiftPreMatchCriteria>(criteria =>
            criteria != null));
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            expectedLeagues,
            expectedDate,
            true);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_WhenStartTimeDoesNotExist_ShouldReturnEarly()
    {
        // Arrange
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;
        _mockRepository.QueryExistedStartTime(expectedDate).Returns((TimeSpan?)null);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryExistedStartTime(expectedDate);
        await _mockRepository.DidNotReceive().QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>());
        await _mockNotificationServiceCommon.DidNotReceive().SendNotificationForNewLeague(
            Arg.Any<IList<string>>(),
            Arg.Any<DateTime>(),
            Arg.Any<bool>());
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleEmptyLeagueList()
    {
        // Arrange
        var emptyLeagues = new List<string>();
        var expectedStartTime = TimeSpan.FromHours(20);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(emptyLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            emptyLeagues,
            expectedDate,
            true);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleRepositoryException_WhenQueryingStartTime()
    {
        // Arrange
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;
        _mockRepository.QueryExistedStartTime(expectedDate)
            .Returns(Task.FromException<TimeSpan?>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SendNotificationForNewLeague());
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleRepositoryException_WhenQueryingAssignments()
    {
        // Arrange
        var expectedStartTime = TimeSpan.FromHours(18);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
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
        var expectedStartTime = TimeSpan.FromHours(18);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
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
        var expectedStartTime = TimeSpan.FromHours(19);
        _mockRepository.QueryExistedStartTime(Arg.Any<DateTime>()).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(expectedLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryExistedStartTime(Arg.Is<DateTime>(date =>
            date == DateTimeOffset.Now.AddHours(12).Date));
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            expectedLeagues,
            Arg.Is<DateTime>(date => date == DateTimeOffset.Now.AddHours(12).Date),
            true);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldPassCorrectParametersToQueryDayShiftPreMatchAssignments()
    {
        // Arrange
        var expectedLeagues = new List<string> { "Test League" };
        var expectedStartTime = TimeSpan.FromHours(21);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns(expectedLeagues);

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryDayShiftPreMatchAssignments(Arg.Is<QueryShiftPreMatchCriteria>(criteria =>
            criteria != null));
    }
}
