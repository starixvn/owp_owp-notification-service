using OWP.PreMatchNotification.LinkedMatch.Models;
using OWP.PreMatchNotification.LinkedMatch.Services;

namespace OWP.PreMatchNotification.UnitTests.LinkedMatch.Services;

public class ChangedLinkedMatchDetectorUnitTests
{
    private readonly ChangedLiveMatchGenerator changedLiveMatchGenerator;
    private readonly ChangedMatchStatusGenerator changedMatchStatusGenerator;
    private readonly ChangedLinkedMatchDetectorImp service;

    public ChangedLinkedMatchDetectorUnitTests()
    {
        changedLiveMatchGenerator = Substitute.For<ChangedLiveMatchGenerator>();
        changedMatchStatusGenerator = Substitute.For<ChangedMatchStatusGenerator>();

        service = new ChangedLinkedMatchDetectorImp(changedLiveMatchGenerator, changedMatchStatusGenerator);
    }

    [Fact]
    public void GetChangedSnapshot_Always_DetectChangedSnapshot()
    {
        var oldMatches = new List<GeneralMatch>
        {
            new(){MatchId=111, InfoSiteMatchId=111},
            new(){MatchId=222, InfoSiteMatchId=222},
            new(){MatchId=333, InfoSiteMatchId=0},
            new(){MatchId=444, InfoSiteMatchId=444,MatchStatus="Live"},
            new(){MatchId=555, InfoSiteMatchId=555} // not contain in prematch
        };
        var newMatches = new List<GeneralMatch>
        {
            new(){MatchId=111, InfoSiteMatchId=222},
            new(){MatchId=222, InfoSiteMatchId=0},
            new(){MatchId=333, InfoSiteMatchId=0},
            new(){MatchId=444, InfoSiteMatchId=444,MatchStatus="Cancel"},
            new(){MatchId=666, InfoSiteMatchId=666} // not contain in prematch
        };

        var deletedMatches = new List<GeneralMatch>()
        {
            new(){MatchId=111, InfoSiteMatchId=111},
            new(){MatchId=222, InfoSiteMatchId=222}
        };
        var insertedMatches = new List<GeneralMatch>()
        {
            new(){MatchId=111, InfoSiteMatchId=222}
        };
        var changedMatches = new List<GeneralMatch>()
        {
            new(){MatchId=444, InfoSiteMatchId=444,MatchStatus="Live"}
        };
        var employees = new Employee()
        {
            PrematchInfosites = new Dictionary<int, List<InfositePrematchLeague>>
            {
                {111, new List<InfositePrematchLeague>()},
                {222, new List<InfositePrematchLeague>()},
                {333, new List<InfositePrematchLeague>()},
                {444, new List<InfositePrematchLeague>()},
            }
        };

        var actual = service.GetChangedSnapshot(oldMatches, newMatches, employees);
        DeepAssert.Equal(deletedMatches, actual.DeletedMatches);
        DeepAssert.Equal(insertedMatches, actual.InsertedMatches);
        DeepAssert.Equal(changedMatches, actual.ChangedMatches);
    }

    [Fact]
    public void Create_Always_ReturnListUninonChangeValue()
    {
        var liveMatchChangedValue = new List<ChangedValue>
        {
            new(){MessageTypeId=200, League="A"}
        };
        var matchStatusChangedValue = new List<ChangedValue>
        {
            new(){MessageTypeId=200, League="A"}
        };
        changedLiveMatchGenerator.Create(Arg.Any<ChangedSnapshot>(), Arg.Any<Employee>())
            .Returns(liveMatchChangedValue);
        changedMatchStatusGenerator.Create(Arg.Any<ChangedSnapshot>(), Arg.Any<Employee>())
            .Returns(matchStatusChangedValue);
        var expected = liveMatchChangedValue.Union(matchStatusChangedValue).ToList();

        var actual = service.Create(new List<GeneralMatch>(), new List<GeneralMatch>(), new Employee());
        DeepAssert.Equal(expected, actual);
    }
}