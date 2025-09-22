using OWP.PreMatchNotification.NewLeague.Criteria;
using OWP.PreMatchNotification.NewLeague.Models;
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
        var expectedLeagues = new List<RCPrematchLeagueInfo>
        {
            new() { LeagueId = 1, LeagueName = "Premier League" },
            new() { LeagueId = 2, LeagueName = "La Liga" },
            new() { LeagueId = 3, LeagueName = "Bundesliga" }
        };
        var expectedInfoSiteMatches = new List<RCInfoSiteMatch>
        {
            new() { LeagueId = 1, IsTest = false },
            new() { LeagueId = 2, IsTest = false },
            new() { LeagueId = 3, IsTest = false }
        };
        var expectedStartTime = TimeSpan.FromHours(18);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((expectedLeagues, expectedInfoSiteMatches));

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryExistedStartTime(expectedDate);
        await _mockRepository.Received(1).QueryDayShiftPreMatchAssignments(Arg.Is<QueryShiftPreMatchCriteria>(criteria =>
            criteria != null));
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            Arg.Is<IList<string>>(leagues => leagues.Count == 3 && leagues.Contains("Premier League") && leagues.Contains("La Liga") && leagues.Contains("Bundesliga")),
            expectedDate,
            false);
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
        var emptyLeagues = new List<RCPrematchLeagueInfo>();
        var emptyInfoSiteMatches = new List<RCInfoSiteMatch>();
        var expectedStartTime = TimeSpan.FromHours(20);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((emptyLeagues, emptyInfoSiteMatches));

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            Arg.Is<IList<string>>(leagues => leagues.Count == 0),
            expectedDate,
            false);
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
            .Returns(Task.FromException<(List<RCPrematchLeagueInfo>, List<RCInfoSiteMatch>)>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SendNotificationForNewLeague());
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleNotificationServiceException()
    {
        // Arrange
        var expectedLeagues = new List<RCPrematchLeagueInfo>
        {
            new() { LeagueId = 1, LeagueName = "Premier League" }
        };
        var expectedInfoSiteMatches = new List<RCInfoSiteMatch>
        {
            new() { LeagueId = 1, IsTest = false }
        };
        var expectedStartTime = TimeSpan.FromHours(18);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((expectedLeagues, expectedInfoSiteMatches));
        _mockNotificationServiceCommon.SendNotificationForNewLeague(Arg.Any<IList<string>>(), Arg.Any<DateTime>(), Arg.Any<bool>())
            .Returns(Task.FromException(new Exception("Notification service error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SendNotificationForNewLeague());
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldUseCorrectDateCalculation()
    {
        // Arrange
        var expectedLeagues = new List<RCPrematchLeagueInfo>
        {
            new() { LeagueId = 1, LeagueName = "Test League" }
        };
        var expectedInfoSiteMatches = new List<RCInfoSiteMatch>
        {
            new() { LeagueId = 1, IsTest = false }
        };
        var expectedStartTime = TimeSpan.FromHours(19);
        _mockRepository.QueryExistedStartTime(Arg.Any<DateTime>()).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((expectedLeagues, expectedInfoSiteMatches));

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryExistedStartTime(Arg.Is<DateTime>(date =>
            date == DateTimeOffset.Now.AddHours(12).Date));
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            Arg.Is<IList<string>>(leagues => leagues.Count == 1 && leagues.Contains("Test League")),
            Arg.Is<DateTime>(date => date == DateTimeOffset.Now.AddHours(12).Date),
            false);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldPassCorrectParametersToQueryDayShiftPreMatchAssignments()
    {
        // Arrange
        var expectedLeagues = new List<RCPrematchLeagueInfo>
        {
            new() { LeagueId = 1, LeagueName = "Test League" }
        };
        var expectedInfoSiteMatches = new List<RCInfoSiteMatch>
        {
            new() { LeagueId = 1, IsTest = false }
        };
        var expectedStartTime = TimeSpan.FromHours(21);
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryExistedStartTime(expectedDate).Returns(expectedStartTime);
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((expectedLeagues, expectedInfoSiteMatches));

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockRepository.Received(1).QueryDayShiftPreMatchAssignments(Arg.Is<QueryShiftPreMatchCriteria>(criteria =>
            criteria != null));
    }
}
