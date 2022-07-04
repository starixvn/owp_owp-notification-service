using OWP.Core.Extensions;
using OWP.OtherSportNotification.Constants;

namespace OWP.OtherSportNotification.Models;

public class MatchSetting
{
    public int OSMatchId { get; set; }

    public string BetTypeName { get; set; }

    public decimal? MaxBet { get; set; }

    public decimal? Spread { get; set; }
}


public class MatchAssignment
{
    public int OSMatchId { get; set; }

    public string BetTypeName { get; set; }

    public int AssignmentType { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; }

    public string Nickname { get; set; }

    public string AssignmentNote { get; set; } = string.Empty;

    public int Order
    {
        get => BettypeOrder.GetValue(BetTypeName, 0);
    }

    private readonly Dictionary<string, int> BettypeOrder = new Dictionary<string, int>
        {
            { Bettype.MatchWinner, -1 },
            { Bettype.FTHDP, 1 },
            { Bettype.FirstHHDP, 2 },
            { Bettype.FirstQHDP, 3 },
            { Bettype.FirstQ3QHDP, 4 },
            { Bettype.FTOUOE, 5 },
            { Bettype.FirstHOUOE, 6 },
            { Bettype.FirstQ3QOUOE, 7 },
            { Bettype.First2ndPeriodsHDP, 8 },
            { Bettype.FTOsU, 9 },
            { Bettype.First2ndPeriodsOU, 10 },
            { Bettype.FirstHOsU, 11 },
            { Bettype.FTOU, 12 },
            { Bettype.FirstHOU, 13 },
            { Bettype.FirstQOU, 14 },
            { Bettype.FirstQ3QOU, 15 },
            { Bettype.LegsHDP, 16 },
            { Bettype.Most180sHDP, 17 },
            { Bettype.MatchWinnerHDP, 18 },
            { Bettype.RacksWinnerHDP, 19 },
            { Bettype.SetWinnerHDP, 20 },
            { Bettype.SetXWinnerHDP, 21 },
            { Bettype.SetWinnerHDPOE, 22 },
            { Bettype.SetsXPointWinnerHDP, 23 },
            { Bettype.MatchWinnerInningsOU, 24 },
            { Bettype.NextOverOUPlayerOU, 25 },
            { Bettype.FT1X2, 26 },
            { Bettype.MatchWinnerHDPOU, 27 },
            { Bettype.FramesWinnerHDPOUOE, 28 },
            { Bettype.First5FramesHDP, 29 },
            { Bettype.First8FramesHDP, 30 },
            { Bettype.First9FramesHDP, 31 },
            { Bettype.FTRunLineHDP, 32 },
            { Bettype.FirstH1st5inningsHDP, 33 },
            { Bettype.FTTotalRunOU, 34 },
            { Bettype.FirstH1st5inningsOU, 35 },
            { Bettype.InningHitsAndRuns, 36 },
            { Bettype.AllProductSetWinner, 37 },
            { Bettype.AllProductPointWinner, 38 },
            { Bettype.RacetoPoints7911, 39 },
            { Bettype.NextPointWinner, 40 },
            { Bettype.MatchHandicap, 41 },
            { Bettype.AllProductLS, 42 },
            { Bettype.NextGame, 43 },
            { Bettype.MostAces, 44 },
            { Bettype.MostDoubleFaults, 45 }
        };
}
