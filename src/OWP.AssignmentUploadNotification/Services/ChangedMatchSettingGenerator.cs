namespace OWP.AssignmentUploadNotification.Services;

using Constants;
using Core.Utils;
using Extensions;
using Models;
using OWP.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

public interface ChangedMatchSettingGenerator
{
    List<ChangedValue> Create(ChangedMatchSnapshot snapshot, int matchId, List<Assignee> existingAssignees);
}

public class ChangedMatchSettingGeneratorImp : ChangedMatchSettingGenerator
{
    private const string EVENT_VS = " -vs- ";

    public List<ChangedValue> Create(ChangedMatchSnapshot snapshot, int matchId, List<Assignee> existingAssignees)
    {
        if (IsSpecific10Mins(snapshot.NewMatch.MinorBettypeName))
        {
            return [];
        }

        var newMatchSettings = snapshot.NewSnapshot.MatchSettings.GetValue(matchId, []);
        var oldMatchSettings = snapshot.OldSnapshot.MatchSettings.GetValue(matchId, []);
        var changedList = GetListValueChangedWithDifferentBettype(oldMatchSettings, newMatchSettings);

        var changedValueList = new List<ChangedValue>();

        foreach (var changed in changedList)
        {
            var mainMatch = snapshot.NewSnapshot.Matches.GetValue(snapshot.NewMatch.ParentMatchId, snapshot.NewMatch);
            var bettypeName = BettypeNameExtensions.GetBettypeNameForMatchSetting(changed.BetTypeId, snapshot.NewMatch.MatchType, snapshot.NewMatch.MinorBettypeName);
            var changedValues = existingAssignees.Select(a => new ChangedValue
            {
                MessageTypeId = changed.MessageTypeId,
                NewValue = changed.NewValue,
                OldValue = changed.OldValue,
                BetTypeName = bettypeName,
                MatchNo = mainMatch.MatchNo,
                League = mainMatch.LeagueName,
                EventName = mainMatch.Home + EVENT_VS + mainMatch.Away,
                AssigneeId = a.AssigneeId,
                AssigneeName = a.AssigneeName
            });

            changedValueList.AddRange(changedValues);
        }

        return changedValueList;
    }

    private static bool IsSpecific10Mins(string bettypeName)
    {
        return !string.IsNullOrEmpty(bettypeName)
               && bettypeName.IndexOf("SPECIFIC 10 MINS", StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    public IEnumerable<ChangedValue> GetListValueChangedWithDifferentBettype(List<MatchSetting> oldMatchSettings, List<MatchSetting> newMatchSettings)
    {
        var majorBettypeGenerateList = new List<int> {
            (int)MajorBetType.Handicap,
            (int)MajorBetType.Handicap1H,
            (int)MajorBetType.FastMarket,
            (int)MajorBetType.LA,
            (int)MajorBetType.XMins,
            (int)MajorBetType.OneXTwo,
            (int)MajorBetType.OneXTwo1H,
            (int)MajorBetType.OverUnder,
            (int)MajorBetType.OverUnder1H
        };

        var changedValues = new List<ChangedValue>();
        foreach (var bettype in majorBettypeGenerateList)
        {
            var oldMatchWithBettype = oldMatchSettings.SingleOrDefault(a => a.BetType == bettype) ?? new MatchSetting();
            var newMatchWithBettype = newMatchSettings.SingleOrDefault(a => a.BetType == bettype) ?? new MatchSetting();
            var listMessage = GetListValueChangedOfMatchSetting(oldMatchWithBettype, newMatchWithBettype, bettype);
            changedValues.AddRange(listMessage);
        }

        return changedValues;
    }

    public IEnumerable<ChangedValue> GetListValueChangedOfMatchSetting(MatchSetting oldMatchSetting, MatchSetting newMatchSetting, int bettype)
    {
        if (!Equals(oldMatchSetting.MaxBet, newMatchSetting.MaxBet))
        {
            yield return new ChangedValue
            {
                OldValue = oldMatchSetting.MaxBet.ToOWPMaxBetFormat(),
                NewValue = newMatchSetting.MaxBet.ToOWPMaxBetFormat(),
                BetTypeId = bettype,
                MessageTypeId = (int)MsgTemplateCode.EditMaxbet
            };
        }

        if (!Equals(oldMatchSetting.Spread, newMatchSetting.Spread))
        {
            yield return new ChangedValue
            {
                OldValue = ToSpreadFormat(oldMatchSetting.Spread),
                NewValue = ToSpreadFormat(newMatchSetting.Spread),
                BetTypeId = bettype,
                MessageTypeId = (int)MsgTemplateCode.EditSpread
            };
        }
    }

    private static string ToSpreadFormat(decimal? spread)
    {
        var spreadFormat = spread.ToOWPSpreadFormat();
        return spreadFormat != string.Empty ? spreadFormat + "%" : spreadFormat;
    }
}