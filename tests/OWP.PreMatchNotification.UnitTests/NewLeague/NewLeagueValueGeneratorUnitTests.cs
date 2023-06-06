using OWP.PreMatchNotification.NewLeague.Models;
using OWP.PreMatchNotification.NewLeague.Services;

namespace OWP.PreMatchNotification.UnitTests.NewLeague;

public class NewLeagueValueGeneratorUnitTests
{
    [Fact]
    public void Create_ReturnNewLeagueMessages()
    {
        var dateTimeHelper = Substitute.For<DateTimeHelper>();

        dateTimeHelper.GetCurrentDate().Returns(new DateTime(2021, 06, 21));
        var service = new NewLeagueValueGeneratorImp(dateTimeHelper);
        var receivedUserIds = MockAssignees().Select(x => x.AssigneeId).ToList();
        var expected = new List<ChangedValue>
        {
            new()
            {
                League = "E",
                ReceivedUserIds = receivedUserIds,
                MessageTypeId = (int)MsgTemplateCode.NewLeague
            }
        };

        var actual = service.Create(MockOldSnapshot(), MockNewSnapshot(), MockAssignees(), MockInfoSiteMatches());

        DeepAssert.Equal(expected, actual.ToList());
    }

    private IList<InfoSiteMatch> MockInfoSiteMatches()
        => new List<InfoSiteMatch>
        {
            new()
            {
                EventDateTime = new DateTime(2021, 06, 21, 02, 00, 00),
                OWPLeagueId = 1
            },
            new()
            {
                EventDateTime = new DateTime(2021, 06, 21, 03, 00, 00),
                OWPLeagueId = 4
            },
            new()
            {
                EventDateTime = new DateTime(2021, 06, 21, 08, 30, 00),
                OWPLeagueId = 4
            }
            ,
            new()
            {
                EventDateTime = new DateTime(2021, 06, 21, 21, 30, 00),
                OWPLeagueId = 5
            }
        };

    private IList<Assignee> MockAssignees()
        => new List<Assignee>
        {
            new()
            {
                AssigneeId = 1,
                AssigneeName = "sa"
            },
            new()
            {
                AssigneeId = 2,
                AssigneeName = "starix1"
            }
        };

    private IList<PreMatchLeague> MockOldSnapshot()
        => new List<PreMatchLeague>
        {
            new()
            {
                PreMatchLeagueId = 1,
                OWPLeagueId = 1,
                IsDay = true,
                OWPLeagueName = "A"
            },
            new()
            {
                PreMatchLeagueId = 2,
                OWPLeagueId = 1,
                IsDay = false,
                OWPLeagueName = "A"
            },
            new()
            {
                PreMatchLeagueId = 3,
                OWPLeagueId = 2,
                IsDay = true,
                OWPLeagueName = "B"
            },
            new()
            {
                PreMatchLeagueId = 4,
                OWPLeagueId = 2,
                IsDay = false,
                OWPLeagueName = "B"
            },
            new()
            {
                PreMatchLeagueId = 5,
                OWPLeagueId = 3,
                IsDay = false,
                OWPLeagueName = "C"
            }
        };

    private IList<PreMatchLeague> MockNewSnapshot()
        => new List<PreMatchLeague>
        {
            new()
            {
                PreMatchLeagueId = 1,
                OWPLeagueId = 1,
                IsDay = true,
                OWPLeagueName = "A"
            },
            new()
            {
                PreMatchLeagueId = 2,
                OWPLeagueId = 1,
                IsDay = false,
                OWPLeagueName = "A"
            },
            new()
            {
                PreMatchLeagueId = 3,
                OWPLeagueId = 2,
                IsDay = true,
                OWPLeagueName = "B"
            },
            new()
            {
                PreMatchLeagueId = 4,
                OWPLeagueId = 2,
                IsDay = false,
                OWPLeagueName = "B"
            },
            new()
            {
                PreMatchLeagueId = 5,
                OWPLeagueId = 3,
                IsDay = false,
                OWPLeagueName = "C"
            },
            new()
            {
                PreMatchLeagueId = 6,
                OWPLeagueId = 3,
                IsDay = true,
                OWPLeagueName = "C"
            },
            new()
            {
                PreMatchLeagueId = 7,
                OWPLeagueId = 4,
                IsDay = false,
                OWPLeagueName = "D"
            },
            new()
            {
                PreMatchLeagueId = 8,
                OWPLeagueId = 4,
                IsDay = true,
                OWPLeagueName = "D"
            },
            new()
            {
                PreMatchLeagueId = 9,
                OWPLeagueId = 5,
                IsDay = true,
                OWPLeagueName = "E"
            }
        };
}