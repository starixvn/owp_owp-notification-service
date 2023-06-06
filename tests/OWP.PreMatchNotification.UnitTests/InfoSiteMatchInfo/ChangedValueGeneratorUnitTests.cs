using OWP.PreMatchNotification.InfoSiteMatchInfo.Models;
using OWP.PreMatchNotification.InfoSiteMatchInfo.Services;

namespace OWP.PreMatchNotification.UnitTests.InfoSiteMatchInfo;

public class ChangedValueGeneratorUnitTests
{
    private readonly ChangedValueGenerator service;

    public ChangedValueGeneratorUnitTests()
    {
        var dateTimeHelper = Substitute.For<DateTimeHelper>();
        dateTimeHelper.GetCurrentDate().Returns(new DateTime(2021, 06, 23));
        service = new ChangedValueGeneratorImp(dateTimeHelper);
    }

    [Fact]
    public void CreateAssigneeMessages_ChangedSnapshot_ReturnChangedValue()
    {
        var expected = GetExpectedAssigneeMessages();

        var actual = service.CreateAssigneeMessages(MockOldSnapshot(), MockNewSnapshot(), MockPreMatchAssignees());

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void CreateLSTMessages_ChangedSnapshot_ReturnChangedValue()
    {
        var expected = GetExpectedLSTMessages();

        var actual = service.CreateLSTMessages(GetExpectedAssigneeMessages(), MockAssigneeLST(), MockLinkedMatHome());

        DeepAssert.Equal(expected, actual);
    }

    private List<ChangedValue> GetExpectedAssigneeMessages()
    {
        return new List<ChangedValue>
        {
            new()
            {
                InfoSiteMatchId = 2,
                League="Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue =  new DateTime(2021, 06, 23, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                NewValue =  new DateTime(2021, 06, 24, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                ReceivedUserIds = new List<int> { 1,4,5 },
                MessageTypeId = (int)MsgTemplateCode.UpdateKOTime
            },
            new()
            {
                InfoSiteMatchId = 2,
                League="Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue = "Liv",
                NewValue = "Liv changed",
                ReceivedUserIds = new List<int> { 1,4,5 },
                MessageTypeId = (int)MsgTemplateCode.UpdateHome
            },
            new()
            {
                InfoSiteMatchId = 2,
                League="Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue = "MC",
                NewValue = "MC changed",
                ReceivedUserIds = new List<int> { 1,4,5 },
                MessageTypeId = (int)MsgTemplateCode.UpdateAway
            },
            new()
            {
                InfoSiteMatchId = 4,
                League="Premier League",
                Home = "Home",
                Away = "TOT",
                OldValue = "TOT",
                NewValue = "TOT changed",
                ReceivedUserIds = new List<int> { 2,3 },
                MessageTypeId = (int)MsgTemplateCode.UpdateAway
            }
        };
    }

    private IList<InfoSiteMatch> MockOldSnapshot()
    {
        return new List<InfoSiteMatch>
        {
            new()
            {
                InfoSiteMatchId = 1,
                HomeName = "MC",
                AwayName = "MU",
                EventDateTime = new DateTime(2021, 06, 23, 23, 00, 00),
                OWPLeagueId = 101,
                LeagueName="Premier League"
            },
            new()
            {
                InfoSiteMatchId = 2,
                HomeName = "Liv",
                AwayName = "MC",
                EventDateTime = new DateTime(2021, 06, 23, 15, 00, 00),
                OWPLeagueId = 101,
                LeagueName="Premier League"
            },
            new()
            {
                InfoSiteMatchId = 3,
                HomeName = "BVB",
                AwayName = "BAY",
                EventDateTime = new DateTime(2021, 06, 24, 03, 59, 21),
                OWPLeagueId = 101,
                LeagueName="Premier League"
            },
            new()
            {
                InfoSiteMatchId = 4,
                HomeName = "Home",
                AwayName = "TOT",
                EventDateTime = new DateTime(2021, 06, 24, 02, 00, 00),
                OWPLeagueId = 102,
                LeagueName="Premier League"
            },
            new()
            {
                InfoSiteMatchId = 7,
                HomeName = "THAI",
                AwayName = "VIET",
                EventDateTime = new DateTime(2021, 06, 24, 10, 00, 00),
                OWPLeagueId = 102,
                LeagueName="AFF"
            }
        };
    }

    private IList<InfoSiteMatch> MockNewSnapshot()
    {
        return new List<InfoSiteMatch>
        {
            new()
            {
                InfoSiteMatchId = 1,
                HomeName = "MC",
                AwayName = "MU",
                EventDateTime = new DateTime(2021, 06, 23, 23, 00, 00),
                OWPLeagueId = 101,
                LeagueName="Premier League"
            },
            new()
            {
                InfoSiteMatchId = 2,
                HomeName = "Liv changed",
                AwayName = "MC changed",
                EventDateTime = new DateTime(2021, 06, 24, 15, 00, 00),
                OWPLeagueId = 101,
                LeagueName="Premier League change"
            },
            new()
            {
                InfoSiteMatchId = 3,
                HomeName = "BVB",
                AwayName = "BAY changed",
                EventDateTime = new DateTime(2021, 06, 24, 03, 59, 21),
                OWPLeagueId = 101,
                LeagueName = "Premier League"
            },
            new()
            {
                InfoSiteMatchId = 4,
                HomeName = "Home",
                AwayName = "TOT changed",
                EventDateTime = new DateTime(2021, 06, 24, 02, 00, 00),
                OWPLeagueId = 102,
                LeagueName = "Premier League "
            },
            new()
            {
                InfoSiteMatchId = 6,
                HomeName = "TOT changed",
                AwayName = "MU changed",
                EventDateTime = new DateTime(2021, 06, 23, 07, 30, 00),
                OWPLeagueId = 102,
                LeagueName = "Premier League"
            }
        };
    }

    private IList<Assignee> MockPreMatchAssignees()
    {
        return new List<Assignee>
        {
            new()
            {
                AssigneeId = 1,
                OWPLeagueId = 101
            },
            new()
            {
                AssigneeId = 2,
                OWPLeagueId = 102
            },
            new()
            {
                AssigneeId = 3,
                OWPLeagueId = 102
            },
            new()
            {
                AssigneeId = 4,
                OWPLeagueId = 101
            },
            new()
            {
                AssigneeId = 5,
                OWPLeagueId = 101
            }
        };
    }

    private IList<ChangedValue> GetExpectedLSTMessages()
    {
        return new List<ChangedValue>
        {
            new()
            {
                InfoSiteMatchId = 2,
                League = "Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue = new DateTime(2021, 06, 23, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                NewValue = new DateTime(2021, 06, 24, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                ReceivedUserIds = new List<int> { 2, 3, 4, 5, 6 },
                MessageTypeId = (int)MsgTemplateCode.UpdateKOTime
            },
            new()
            {
                InfoSiteMatchId = 2,
                League = "Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue = "Liv",
                NewValue = "Liv changed",
                ReceivedUserIds = new List<int> { 2, 3, 4, 5, 6 },
                MessageTypeId = (int)MsgTemplateCode.UpdateHome
            },
            new()
            {
                InfoSiteMatchId = 2,
                League = "Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue = "MC",
                NewValue = "MC changed",
                ReceivedUserIds = new List<int> { 2, 3, 4, 5, 6 },
                MessageTypeId = (int)MsgTemplateCode.UpdateAway
            }
        };
    }

    private IList<Assignee> MockAssigneeLST()
    {
        return new List<Assignee>
        {
            new()
            {
                AssigneeId = 2
            },
            new()
            {
                AssigneeId = 3
            },
            new()
            {
                AssigneeId = 4
            },
            new()
            {
                AssigneeId = 5
            },
            new()
            {
                AssigneeId = 6
            }
        };
    }

    private IList<InfoSiteMatch> MockLinkedMatHome()
    {
        return new List<InfoSiteMatch>
        {
            new()
            {
                InfoSiteMatchId = 1
            },
            new()
            {
                InfoSiteMatchId = 2
            },
            new()
            {
                InfoSiteMatchId = 3
            },
            new()
            {
                InfoSiteMatchId = 5
            }
        };
    }
}