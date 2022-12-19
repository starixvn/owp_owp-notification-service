namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Extensions
{
    using OWP.AssignmentUploadNotification.Constants;
    using OWP.AssignmentUploadNotification.Extensions;
    using Xunit;

    public class BettypeNameExtensionsUnitTests
    {
        [Theory]
        [InlineData((int)MajorBetType.LA, 0, "", "LA")]
        [InlineData((int)MajorBetType.Handicap, 0, "", "HDP")]
        [InlineData((int)MajorBetType.OverUnder, 0, "", "O/U")]
        [InlineData((int)MajorBetType.OverUnder1H, 0, "", "1H O/U")]

        [InlineData(0, (int)MinorBettype.Specific15Mins, "SPECIFIC 15 MINS", "SPECIFIC 15 MINS")]
        [InlineData(0, (int)MinorBettype.Specific10Mins, "SPECIFIC 10 MINS", "SPECIFIC 10 MINS")]
        [InlineData(0, (int)MinorBettype.FastMarket, "Fast Market", "Fast Market")]
        [InlineData(0, (int)MinorBettype.AllInOne, "All In One", "All In One")]

        [InlineData((int)MajorBetType.Handicap1H, (int)MinorBettype.NoOfCorner, "No.of Corners", "No.of Corners (1H HDP)")]
        [InlineData((int)MajorBetType.OneXTwo, -92, "Home Single Team O/U", "Home Single Team O/U (FT.1X2)")]
        public void GetBettypeName_Always_ReturnBettypeName(int bettypeId, int minorBettypeId, string minorBettypeName, string expected)
        {
            var actual = BettypeNameExtensions.GetBettypeNameForAssignment(bettypeId, minorBettypeId, minorBettypeName);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((int)MajorBetType.LA, 0, "", "LA")]
        [InlineData((int)MajorBetType.Handicap, 0, "", "HDP")]
        [InlineData((int)MajorBetType.OverUnder, 0, "", "O/U")]
        [InlineData((int)MajorBetType.OverUnder1H, 0, "", "1H O/U")]

        [InlineData(0, (int)MinorBettype.Specific15Mins, "", "X MIN")]
        [InlineData(0, (int)MinorBettype.FastMarket, "", "Fast Market")]

        [InlineData((int)MajorBetType.Handicap1H, (int)MinorBettype.NoOfCorner, "No.of Corners", "No.of Corners (1H HDP)")]
        [InlineData((int)MajorBetType.OneXTwo, -92, "Home Single Team O/U", "Home Single Team O/U (FT.1X2)")]
        public void GetBettypeNameForMatchSetting_Always_ReturnBettypeName(int bettypeId, int minorBettypeId, string minorBettypeName, string expected)
        {
            var actual = BettypeNameExtensions.GetBettypeNameForMatchSetting(bettypeId, minorBettypeId, minorBettypeName);
            Assert.Equal(expected, actual);
        }
    }
}