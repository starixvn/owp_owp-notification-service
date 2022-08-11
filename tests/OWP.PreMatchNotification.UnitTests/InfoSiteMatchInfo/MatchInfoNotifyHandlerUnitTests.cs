using OWP.Core.NotificationRequests.Prematch;
using OWP.PreMatchNotification.InfoSiteMatchInfo;
using OWP.PreMatchNotification.InfoSiteMatchInfo.Models;
using OWP.PreMatchNotification.InfoSiteMatchInfo.Services;

namespace OWP.PreMatchNotification.UnitTests.InfoSiteMatchInfo;

public class MatchInfoNotifyHandlerUnitTests
{
    [Fact]
    public async Task GenerateMessages_ReturnChangedValues()
    {
        var service = MockService();
        var expected = MockexpectedAssigneeMessages().Union(MockExpectedLSTMessages()).ToList();

        var actual = await service.GenerateMessages(new MatchInfoNotifyRequest());

        DeepAssert.Equal(expected, actual);
    }

    private MatchInfoNotifyHandler MockService()
    {
        var cacheManipulator = Substitute.For<CacheManipulatorCommon>();
        var queryRepository = Substitute.For<Repository>();
        var changedValueGenerator = Substitute.For<ChangedValueGenerator>();
        var messageFactory = Substitute.For<MessageFactory>();
        var commandRepository = Substitute.For<CommandRepository>();
        var notify = Substitute.For<NotificationService>();

        cacheManipulator.GetMultipleData<InfoSiteMatch>(Arg.Any<string>()).Returns(MockOldSnapshot());
        queryRepository.GetInfoSiteMatchNewSnapshot(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(MockNewSnapshot());
        queryRepository.GetLGALinkedInfoSiteMatch(Arg.Any<List<int>>()).Returns(MockLinkedMatches());
        commandRepository
            .GetPreMatchLeagueAssigneesByOWPLeagueIds(Arg.Any<List<int>>(), Arg.Any<DateTime>()).Returns(MockPreMatchAssignees());
        commandRepository.GetEmployeesBySection(Arg.Any<string>()).Returns(MockAssigneeLST());
        changedValueGenerator
            .CreateAssigneeMessages(Arg.Any<List<InfoSiteMatch>>(), Arg.Any<List<InfoSiteMatch>>(), Arg.Any<List<Assignee>>())
            .Returns(MockexpectedAssigneeMessages());
        changedValueGenerator
            .CreateLSTMessages(Arg.Any<List<ChangedValue>>(), Arg.Any<List<Assignee>>(), Arg.Any<List<InfoSiteMatch>>())
            .Returns(MockExpectedLSTMessages());

        return new MatchInfoNotifyHandler(cacheManipulator,
                                            queryRepository,
                                            changedValueGenerator,
                                            messageFactory,
                                            commandRepository,
                                            notify);
    }

    private IList<ChangedValue> MockexpectedAssigneeMessages()
    {
        return new List<ChangedValue>
        {
            new ChangedValue
            {
                InfoSiteMatchId = 1,
                League="Premier League",
                Home = "MC",
                Away = "MU",
                OldValue = "MC" ,
                NewValue =  "MC changed",
                ReceivedUserIds = new List<int> { 1,4,5 },
                MessageTypeId = (int)MsgTemplateCode.UpdateHome
            },
            new ChangedValue
            {
                InfoSiteMatchId = 2,
                League="Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue =  new DateTime(2021, 06, 23, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                NewValue =  new DateTime(2022, 09, 03, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                ReceivedUserIds = new List<int> { 1,4,5 },
                MessageTypeId = (int)MsgTemplateCode.UpdateKOTime
            }
        };
    }

    private IList<InfoSiteMatch> MockOldSnapshot()
    {
        return new List<InfoSiteMatch>
        {
            new InfoSiteMatch
            {
                InfoSiteMatchId = 1,
                HomeName = "MC",
                AwayName = "MU",
                EventDateTime = new DateTime(2021, 06, 23, 23, 00, 00),
                OWPLeagueId = 101,
                LeagueName="Premier League"
            },
            new InfoSiteMatch
            {
                InfoSiteMatchId = 2,
                HomeName = "Liv",
                AwayName = "MC",
                EventDateTime = new DateTime(2021, 06, 23, 15, 00, 00),
                OWPLeagueId = 101,
                LeagueName="Premier League"
            }
        };
    }

    private IList<InfoSiteMatch> MockNewSnapshot()
    {
        return new List<InfoSiteMatch>

        {
            new InfoSiteMatch
            {
                InfoSiteMatchId = 1,
                HomeName = "MC changed",
                AwayName = "MU",
                EventDateTime = new DateTime(2021, 06, 23, 23, 00, 00),
                OWPLeagueId = 101
            },
            new InfoSiteMatch
            {
                InfoSiteMatchId = 2,
                HomeName = "Liv changed",
                AwayName = "MC changed",
                EventDateTime = new DateTime(2022, 09, 03, 15, 00, 00),
                OWPLeagueId = 101,
                LeagueName="Premier League change"
            }
        };
    }

    private IList<Assignee> MockPreMatchAssignees()
    {
        return new List<Assignee>
        {
            new Assignee
            {
                AssigneeId = 1,
                OWPLeagueId = 101
            },
            new Assignee
            {
                AssigneeId = 2,
                OWPLeagueId = 102
            },
            new Assignee
            {
                AssigneeId = 3,
                OWPLeagueId = 102
            },
            new Assignee
            {
                AssigneeId = 4,
                OWPLeagueId = 101
            },
            new Assignee
            {
                AssigneeId = 5,
                OWPLeagueId = 101
            }
        };
    }

    private IList<ChangedValue> MockExpectedLSTMessages()
    {
        return new List<ChangedValue>
        {
            new ChangedValue
            {
                InfoSiteMatchId = 1,
                League="Premier League",
                Home = "MC",
                Away = "MU",
                OldValue = "MC" ,
                NewValue =  "MC changed",
                ReceivedUserIds = new List<int> { 2, 3, 4, 5, 6 },
                MessageTypeId = (int)MsgTemplateCode.UpdateHome
            }
        };
    }

    private IList<Assignee> MockAssigneeLST()
    {
        return new List<Assignee>
        {
            new Assignee
            {
                AssigneeId = 2
            },
            new Assignee
            {
                AssigneeId = 3
            },
            new Assignee
            {
                AssigneeId = 4
            },
            new Assignee
            {
                AssigneeId = 5
            },
            new Assignee
            {
                AssigneeId = 6
            },
        };
    }

    private IList<InfoSiteMatch> MockLinkedMatches()
    {
        return new List<InfoSiteMatch>
        {
            new InfoSiteMatch
            {
                InfoSiteMatchId = 1
            },
            new InfoSiteMatch
            {
                InfoSiteMatchId = 2
            },
            new InfoSiteMatch
            {
                InfoSiteMatchId = 3
            },
            new InfoSiteMatch
            {
                InfoSiteMatchId = 5
            }
        };
    }
}