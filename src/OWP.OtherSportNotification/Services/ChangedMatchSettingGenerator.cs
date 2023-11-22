using OWP.Core.Extensions;
using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Helpers;
using OWP.OtherSportNotification.Models;
using System.Globalization;

namespace OWP.OtherSportNotification.Services;

public interface ChangedMatchSettingGenerator
{
    List<ChangedValue> Create(ChangedMatchSnapshot snapshot, int matchId, List<Assignee> existingAssignees);
}

public class ChangedMatchSettingGeneratorImp : ChangedMatchSettingGenerator
{
    public List<ChangedValue> Create(ChangedMatchSnapshot snapshot, int matchId, List<Assignee> existingAssignees)
    {
        var newMatchSettings = snapshot.NewSnapshot.MatchSettings.GetValue(matchId, new List<MatchSetting>());
        var oldMatchSettings = snapshot.OldSnapshot.MatchSettings.GetValue(matchId, new List<MatchSetting>());
        var changedList = GetListValueChangedWithDifferentBettype(oldMatchSettings, newMatchSettings, matchId);

        var changedValueList = new List<ChangedValue>();

        foreach (var changed in changedList)
        {
            var matchInfo = snapshot.NewSnapshot.Matches.GetValue(changed.OSMatchId, snapshot.NewMatch);
            var changedValues = existingAssignees.Select(a => new ChangedValue
            {
                MessageTypeId = changed.MessageTypeId,
                NewValue = changed.NewValue,
                OldValue = changed.OldValue,
                BetTypeName = changed.BetTypeName,
                MatchNo = $"{matchInfo.MatchNo}",
                AdminsiteMatchID = matchInfo.AdminsiteMatchId,
                League = matchInfo.League,
                EventName = UpdatedValueHelper.CreateEventName(matchInfo),
                UserId = a.AssigneeId,
                Username = a.AssigneeName,
                OSMatchId = matchId
            });

            changedValueList.AddRange(changedValues);
        }

        return changedValueList;
    }

    public static IEnumerable<ChangedValue> GetListValueChangedWithDifferentBettype(List<MatchSetting> oldMatchSettings, List<MatchSetting> newMatchSettings, int matchId)
    {
        var betTypes = new List<string> {
             Bettype.FTHDP,
             Bettype.FirstHHDP,
             Bettype.FirstQ3QHDP ,
             Bettype.FTOUOE ,
             Bettype.FTOE,
             Bettype.FirstHOUOE ,
             Bettype.FirstHOE,
             Bettype.FirstHOU ,
             Bettype.FirstHOsU ,
             Bettype.FirstQ3QOUOE ,
             Bettype.FirstQ3QOE,
             Bettype.FirstQ3QOU,
             Bettype.FirstQHDP ,
             Bettype.FirstQOU ,
             Bettype.FTOU ,
             Bettype.FTOsU ,
             Bettype.First2ndPeriodsHDP ,
             Bettype.First2ndPeriodsOU ,
             Bettype.MatchWinner ,
             Bettype.LegsHDP ,
             Bettype.Most180sHDP ,
             Bettype.MatchWinnerHDP ,
             Bettype.RacksWinnerHDP ,
             Bettype.SetWinnerHDP ,
             Bettype.SetXWinnerHDP ,
             Bettype.SetWinnerHDPOE ,
             Bettype.SetWinnerOE,
             Bettype.SetsXPointWinnerHDP ,
             Bettype.MatchWinnerInningsOU ,
             Bettype.NextOverOUPlayerOU ,
             Bettype.FT1X2 ,
             Bettype.MatchWinnerHDPOU ,
             Bettype.MatchWinnerOU,
             Bettype.FramesWinnerHDPOUOE ,
             Bettype.FramesWinnerOU,
             Bettype.FramesWinnerOE,
             Bettype.First5FramesHDP ,
             Bettype.First8FramesHDP,
             Bettype.First9FramesHDP ,
             Bettype.FTRunLineHDP ,
             Bettype.FirstH1st5inningsHDP ,
             Bettype.FTTotalRunOU ,
             Bettype.FirstH1st5inningsOU ,
             Bettype.InningHitsAndRuns ,
             Bettype.AllProductSetWinner ,
             Bettype.AllProductPointWinner ,
             Bettype.RacetoPoints7911 ,
             Bettype.NextPointWinner ,
             Bettype.MatchHandicap ,
             Bettype.AllProductLS ,
             Bettype.NextGame ,
             Bettype.MostAces ,
             Bettype.MostDoubleFaults
        };

        var changedValues = new List<ChangedValue>();
        foreach (var bettype in betTypes)
        {
            var oldMatchWithBettype = oldMatchSettings.SingleOrDefault(a => string.Equals(a.BetTypeName, bettype)) ?? new MatchSetting();
            var newMatchWithBettype = newMatchSettings.SingleOrDefault(a => string.Equals(a.BetTypeName, bettype)) ?? new MatchSetting();
            var listMessage = GetListValueChangedOfMatchSetting(oldMatchWithBettype, newMatchWithBettype, bettype, matchId);
            changedValues.AddRange(listMessage);
        }

        return changedValues;
    }

    public static IEnumerable<ChangedValue> GetListValueChangedOfMatchSetting(MatchSetting oldMatchSetting, MatchSetting newMatchSetting, string bettype, int matchId)
    {
        var changedValueList = new List<ChangedValue>();
        if (!Equals(oldMatchSetting.MaxBet, newMatchSetting.MaxBet))
        {
            changedValueList.Add(new ChangedValue
            {
                OldValue = GetMaxbetSpreadValue(oldMatchSetting.MaxBet) ?? "",
                NewValue = GetMaxbetSpreadValue(newMatchSetting.MaxBet) ?? "",
                BetTypeName = bettype,
                MessageTypeId = (int)MsgTemplateCode.EditMaxbet,
                OSMatchId = matchId
            });
        }
        if (!Equals(oldMatchSetting.Spread, newMatchSetting.Spread))
        {
            changedValueList.Add(new ChangedValue
            {
                OldValue = ToOWPSpreadFormatPercent(oldMatchSetting.Spread),
                NewValue = ToOWPSpreadFormatPercent(newMatchSetting.Spread),
                BetTypeName = bettype,
                MessageTypeId = (int)MsgTemplateCode.EditSpread,
                OSMatchId = matchId
            });
        }
        return changedValueList;
    }

    private static string GetMaxbetSpreadValue(decimal? value)
        => value?.ToString().TrimEnd('0').TrimEnd('.');
    public static string ToOWPSpreadFormatPercent(decimal? spread)
             => spread.HasValue
             ? $"{ToOWPSpreadFormat(spread)}%"
             : string.Empty;
    public static string ToOWPSpreadFormat(decimal? spread)
    {
        const int percent = 100;
        return spread.HasValue ? (spread.Value * percent).ToString("0.##", CultureInfo.InvariantCulture) : string.Empty;
    }
}
