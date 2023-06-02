using OWP.PreMatchNotification.LinkedMatch.Models;
using OWP.PreMatchNotification.LinkedMatch.Services;

namespace OWP.PreMatchNotification.UnitTests.LinkedMatch.Services;

public class ChangedMatchStatusGeneratorUnitTests
{
    private readonly ChangedMatchStatusGeneratorImp service;

    public ChangedMatchStatusGeneratorUnitTests()
    {
        service = new ChangedMatchStatusGeneratorImp();
    }

    [Fact]
    public void Create_ChangedSnapshot_ReturnChangedValue()
    {
        var changedSnapshot = new ChangedSnapshot
        {
            OldSnapshot = new List<GeneralMatch>
            {
                new(){MatchId=111, InfoSiteMatchId=111, MatchStatus="Postponed"},
                new(){MatchId=222, InfoSiteMatchId=222, MatchStatus="Cancel"}
            },
            NewSnapshot = new List<GeneralMatch>
            {
                new(){MatchId=111, InfoSiteMatchId=111, MatchStatus="Live", LeagueName="LeagueA",Home="HomeA",Away="AwayA"},
                new(){MatchId=222, InfoSiteMatchId=222, MatchStatus="Cancel", LeagueName="LeagueB",Home="HomeB",Away="AwayB"}
            },
            ChangedMatches = new List<GeneralMatch>
            {
                new(){MatchId=111, InfoSiteMatchId=111, MatchStatus="Live", LeagueName="LeagueA",Home="HomeA",Away="AwayA"},
                new(){MatchId=222, InfoSiteMatchId=222, MatchStatus="Cancel", LeagueName="LeagueB",Home="HomeB",Away="AwayB"}
            }
        };
        var employees = new Employee
        {
            PrematchInfosites = new Dictionary<int, List<InfositePrematchLeague>>
            {
                {111, new(){ new InfositePrematchLeague { PreMatchLeagueId=100, InfoSiteMatchId=111, PrematchAssigneeIds=new List<int>{8,9}, LeagueName="LeagueA",HomeName="HomeA",AwayName="AwayA" },
                             new InfositePrematchLeague { PreMatchLeagueId=333, InfoSiteMatchId=111, PrematchAssigneeIds=new List<int>{10,11,8} }} },
                {222, new(){ new InfositePrematchLeague { PreMatchLeagueId=200, InfoSiteMatchId=222, PrematchAssigneeIds=new List<int>(), LeagueName="LeagueB",HomeName="HomeB",AwayName="AwayB" } } }
            },
            AllLSTEmployees = new List<int> { 1, 2, 3 }
        };
        var expected = new List<ChangedValue>
        {
            new(){
                MessageTypeId=(int)MsgTemplateCode.LGAChangeMatchStatus,
                League="LeagueA",
                Home="HomeA",
                Away="AwayA",
                OldValue = "Postponed",
                NewValue = "Live",
                ReceivedUserIds=new List<int>{1,2,3,8,9,10,11}
            }
        };
        var actual = service.Create(changedSnapshot, employees);

        DeepAssert.Equal(expected, actual);
    }
}