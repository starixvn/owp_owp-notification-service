namespace OWP.AssignmentUploadNotification.Constants
{
    using System.Collections.Generic;
    using OWP.Core.Extensions;

    public static class MajorBettypeShortName
    {
        private static readonly Dictionary<int, string> majorShortName = new Dictionary<int, string>
        {
            { (int)MajorBetType.AllInOne, "AIO"},
            { (int)MajorBetType.FastMarket, "FM"},
            { (int)MajorBetType.LA, "LA"},
            { (int)MajorBetType.XMins, "XMins"},
            { (int)MajorBetType.Handicap, "HDP"},
            { (int)MajorBetType.OverUnder, "O/U"},
            { (int)MajorBetType.OneXTwo, "FT.1X2"},
            { (int)MajorBetType.Handicap1H, "1H HDP"},
            { (int)MajorBetType.OverUnder1H, "1H O/U"},
            { (int)MajorBetType.OneXTwo1H, "1H 1X2"}
        };

        public static string GetMajorBetTypeShortName(int betType)
        {
            return majorShortName.GetValue(betType, string.Empty);
        }
    }
}
