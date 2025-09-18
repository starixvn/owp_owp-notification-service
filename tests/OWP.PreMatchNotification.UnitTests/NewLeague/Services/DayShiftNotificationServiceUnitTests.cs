using OWP.PreMatchNotification.NewLeague.Criteria;
using OWP.PreMatchNotification.NewLeague.Models;
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
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((expectedLeagues, expectedInfoSiteMatches));

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
        var expectedLeagues = new List<RCPrematchLeagueInfo>
        {
            new() { LeagueId = 1, LeagueName = "Premier League" },
            new() { LeagueId = 2, LeagueName = "La Liga" }
        };
        var expectedInfoSiteMatches = new List<RCInfoSiteMatch>
        {
            new() { LeagueId = 1, IsTest = false },
            new() { LeagueId = 2, IsTest = false }
        };
        var expectedDate = DateTimeOffset.Now.AddHours(12).Date;

        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((expectedLeagues, expectedInfoSiteMatches));

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            Arg.Is<IList<string>>(leagues => leagues.Count == 2 && leagues.Contains("Premier League") && leagues.Contains("La Liga")),
            expectedDate,
            true);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleEmptyLeagueList()
    {
        // Arrange
        var emptyLeagues = new List<RCPrematchLeagueInfo>();
        var emptyInfoSiteMatches = new List<RCInfoSiteMatch>();
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((emptyLeagues, emptyInfoSiteMatches));

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            Arg.Is<IList<string>>(leagues => leagues.Count == 0),
            Arg.Any<DateTime>(),
            true);
    }

    [Fact]
    public async Task SendNotificationForNewLeague_ShouldHandleRepositoryException()
    {
        // Arrange
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
        _mockRepository.QueryDayShiftPreMatchAssignments(Arg.Any<QueryShiftPreMatchCriteria>())
            .Returns((expectedLeagues, expectedInfoSiteMatches));

        // Act
        await _service.SendNotificationForNewLeague();

        // Assert
        await _mockNotificationServiceCommon.Received(1).SendNotificationForNewLeague(
            Arg.Is<IList<string>>(leagues => leagues.Count == 1 && leagues.Contains("Test League")),
            Arg.Is<DateTime>(date => date == DateTimeOffset.Now.AddHours(12).Date),
            true);
    }
}
