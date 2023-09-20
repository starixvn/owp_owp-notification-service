namespace OWP.AssignmentUploadNotification.Extensions;

using Constants;

public static class BettypeNameExtensions
{
    public static string GetBettypeNameForAssignment(int bettypeId, int minorBettypeId, string minorBettypeName)
    {
        if (minorBettypeId == 0)
        {
            return MajorBettypeShortName.GetMajorBetTypeShortName(bettypeId);
        }

        return MinorBettypeConst.GenerateMinorBettypeId.Contains(minorBettypeId)
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