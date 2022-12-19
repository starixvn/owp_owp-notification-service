namespace OWP.AssignmentUploadNotification.Extensions
{
    using System.Collections.Generic;
    using Constants;

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
            switch (minorBettypeId)
            {
                case 0:
                    return MajorBettypeShortName.GetMajorBetTypeShortName(bettypeId);
                case (int)MinorBettype.Specific15Mins:
                    return "X MIN";
                case (int)MinorBettype.FastMarket:
                    return "Fast Market";
                default:
                    return $"{minorBettypeName} ({MajorBettypeShortName.GetMajorBetTypeShortName(bettypeId)})";
            }
        }
    }
}
