namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Sevices
{
    using System.Collections.Generic;
    using OWP.AssignmentUploadNotification.Models;
    using OWP.AssignmentUploadNotification.Services;
    using Xunit;
    using Xunit.Asserts.Compare;

    public class ChangedMatchInfoGeneratorUnitTest
    {
        [Theory]
        [InlineData(-1, true)]
        [InlineData(-2, true)]
        [InlineData(-88, true)]
        [InlineData(-89, true)]
        [InlineData(-5, true)]
        [InlineData(-8, true)]
        [InlineData(-3, false)]
        [InlineData(-6, false)]

        public void IsIgnoredMinorMatch(int matchType, bool expected)
        {
            var actual = ChangedMatchInfoGeneratorImp.IsIgnoredMinorMatch(matchType);
            DeepAssert.Equal(expected, actual);
        }

        [Fact]
        public void Create_IgnoredMinorMatch_ReturnChangedValueList()
        {
            // Arrange
            var snapshot = DataInput();
            var assignments = new List<Assignee>
            {
                new Assignee
                {
                     AssigneeId = 6789,
                     AssigneeName = "esther"
                }
            };

            var changedMatchInfo = new ChangedMatchInfoGeneratorImp();
            var expected = DataExpected();

            // Act
            var actual = changedMatchInfo.Create(snapshot.OldMatch, snapshot.NewMatch, assignments);

            // Assert
            DeepAssert.Equal(expected, actual);
        }

        private ChangedMatchSnapshot DataInput()
        {
            var newMatch = new GeneralMatch
            {
                MatchId = 1234,
                MatchNo = "20000",
                Home = "LiverPool-Coners",
                Away = "Arsenal-Coners",
                LeagueName = "PremierLeague-Coners",
                MatchStatus = "Live",
                Agent = "Rossi",
                ParentMatchId = 3456,
                MatchType = 1000,
                MinorBettypeName = "Next of Coners",
                Division = "20",
                Remark = "ahihihahah",
                WB = ""

            };

            var oldMatch = new GeneralMatch
            {
                MatchId = 1234,
                MatchNo = "1234",
                Home = "LiverPool-Coners",
                Away = "Arsenal-Coners",
                LeagueName = "PremierLeague-Coners",
                MatchStatus = "",
                Agent = "Bow",
                ParentMatchId = 3456,
                MatchType = 1000,
                MinorBettypeName = "Next of Coners",
                Division = "10",
                Remark = "",
                WB = "WB@"
            };

            var mainMatch = new GeneralMatch
            {
                MatchId = 3456,
                MatchNo = "50000",
                Home = "LiverPool",
                Away = "Arsenal",
                LeagueName = "PremierLeague",
                MatchStatus = "Live",
                Agent = "Rossi"

            };
            var matches = new Dictionary<int, GeneralMatch> { { 3456, mainMatch } };

            var newSnapshot = new AssignmentSnapshot
            {
                Matches = matches
            };

            var oldSnapshot = new AssignmentSnapshot
            {
                Matches = matches
            };

            var snapshot = new ChangedMatchSnapshot { NewMatch = newMatch, OldMatch = oldMatch, NewSnapshot = newSnapshot, OldSnapshot = oldSnapshot };
            return snapshot;
        }

        private List<ChangedValue> DataExpected()
        {
            var valueChanged = new ChangedValue
            {
                MessageTypeId = 40,
                NewValue = "20000",
                OldValue = "1234",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther"
            };

            var valueChanged2 = new ChangedValue
            {
                MessageTypeId = 25,
                NewValue = "Rossi",
                OldValue = "Bow",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther"
            };

            var valueChanged3 = new ChangedValue
            {
                MessageTypeId = 22,
                NewValue = "20",
                OldValue = "10",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther"
            };

            var valueChanged4 = new ChangedValue
            {
                MessageTypeId = 26,
                NewValue = "ahihihahah",
                OldValue = "",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther"
            };

            var valueChanged5 = new ChangedValue
            {
                MessageTypeId = 16,
                NewValue = "",
                OldValue = "WB@",
                League = "PremierLeague-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther"
            };

            var changedValueList = new List<ChangedValue>
            {
                valueChanged,
                valueChanged2,
                valueChanged3,
                valueChanged4,
                valueChanged5
            };

            return changedValueList;
        }
    }
}
