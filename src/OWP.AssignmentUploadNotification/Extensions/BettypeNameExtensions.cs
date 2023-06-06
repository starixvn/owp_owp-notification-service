namespace OWP.AssignmentUploadNotification.Extensions;

using Constants;
using System.Collections.Generic;

public static class BettypeNameExtensions
{
    public static string GetBettypeNameForAssignment(int bettypeId, int minorBettypeId, string minorBettypeName)
    {
        var minorGenerateList = new List<int> {
            (int)MinorBettype.Specific15Mins,
            (int)MinorBettype.Specific10Mins,
            (int)MinorBettype.FastMarket,
            (int)MinorBettype.AllInOne,
            (int)MinorBettype.Specific15MinsCorners,
            (int)MinorBettype.Specific15MinsTotalBookings
        };

        if (minorBettypeId == 0)
        {
            return MajorBettypeShortName.GetMajorBetTypeShortName(bettypeId);
        }

        return minorGenerateList.Contains(minorBettypeId)
            ? minorBettypeName
            : $"{minorBettypeName} ({MajorBettypeShortName.GetMajorBetTypeShortName(bettypeId)})";
    }

    public static string GetBettypeNameForMatchSetting(int bettypeId, int minorBettypeId, string minorBettypeName)
    {
        return minorBettypeId switch
        {
            0 => MajorBettypeShortName.GetMajorBetTypeShortName(bettypeId),
            (int)MinorBettype.Specific15Mins => "X MIN",
            (int)MinorBettype.FastMarket => "Fast Market",
            _ => $"{minorBettypeName} ({MajorBettypeShortName.GetMajorBetTypeShortName(bettypeId)})"
        };
    }
}