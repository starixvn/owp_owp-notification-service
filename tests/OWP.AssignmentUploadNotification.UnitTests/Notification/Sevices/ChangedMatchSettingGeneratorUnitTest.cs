namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Sevices
{
    using System.Collections.Generic;
    using OWP.AssignmentUploadNotification.Models;
    using OWP.AssignmentUploadNotification.Services;
    using Xunit;
    using Xunit.Asserts.Compare;

    public class ChangedMatchSettingGeneratorUnitTest
    {
        [Fact]
        public void Create_IsNotSpecific10Mins_ReturnChangedMatchSettingList()
        {
            // Arrange
            var snapshot = DataInputIsNotSpecific10Mins();
            var assignments = new List<Assignee>
            {
                new Assignee
                {
                     AssigneeId = 6789,
                     AssigneeName = "esther"
                }
            };

            var changedMatchSetting = new ChangedMatchSettingGeneratorImp();
            var expected = DataExpected();

            // Act
            var actual = changedMatchSetting.Create(snapshot, 1234, assignments);

            // Assert
            DeepAssert.Equal(expected, actual);
        }

        [Fact]
        public void Create_IsSpecific10Mins_ReturnNewChangedValueList()
        {
            // Arrange
            var snapshot = DataInputIsSpecific10Mins();
            var assignments = new List<Assignee>
            {
                new Assignee
                {
                     AssigneeId = 6789,
                     AssigneeName = "esther"
                }
            };

            var changedMatchSetting = new ChangedMatchSettingGeneratorImp();
            var expected = new List<ChangedValue>();

            // Act
            var actual = changedMatchSetting.Create(snapshot, 1234, assignments);

            // Assert
            DeepAssert.Equal(expected, actual);
        }

        private List<ChangedValue> DataExpected()
        {
            var valueChanged = new ChangedValue
            {
                OldValue = "400K",
                NewValue = "300K",
                BetTypeName = "Next of Coners (HDP)",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                MessageTypeId = 29
            };

            var valueChanged2 = new ChangedValue
            {
                OldValue = "50%",
                NewValue = "70%",
                BetTypeName = "Next of Coners (HDP)",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                MessageTypeId = 30
            };

            var valueChanged3 = new ChangedValue
            {
                OldValue = "",
                NewValue = "30K",
                BetTypeName = "Next of Coners (1H HDP)",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                MessageTypeId = 29
            };

            var valueChanged4 = new ChangedValue
            {
                OldValue = "20K",
                NewValue = "",
                BetTypeName = "Next of Coners (1H HDP)",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                MessageTypeId = 33
            };

            var valueChanged5 = new ChangedValue
            {
                OldValue = "",
                NewValue = "40K",
                BetTypeName = "Next of Coners (FM)",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                MessageTypeId = 33
            };

            var valueChanged6 = new ChangedValue
            {
                OldValue = "",
                NewValue = "50%",
                BetTypeName = "Next of Coners (FM)",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                MessageTypeId = 34
            };

            var changedValueList = new List<ChangedValue>
            {
                valueChanged,
                valueChanged2,
                valueChanged3,
                //valueChanged4,
                //valueChanged5,
                //valueChanged6
            };

            return changedValueList;
        }

        private ChangedMatchSnapshot DataInputIsSpecific10Mins()
        {
            var newMatch = new GeneralMatch
            {
                MatchId = 1234,
                MatchNo = "20000",
                Home = "LiverPool-Coners",
                Away = "Arsenal-Coners",
                LeagueName = "PremierLeague-Coners",
                MatchStatus = "Cancel",
                Agent = "Rossi",
                ParentMatchId = 3456,
                MatchType = -2,
                MinorBettypeName = "Specific 10 Mins",
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
                MatchType = -2,
                MinorBettypeName = "Specific 10 Mins",
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
                MatchStatus = "Cancel",
                Agent = "Rossi"
            };
            var matches = new Dictionary<int, GeneralMatch> { { 3456, mainMatch } };

            var newMatchSettingList = NewMatchSettingList();
            var oldMatchSettingList = OldMatchSettingList();

            var matchSettings = new Dictionary<int, List<MatchSetting>> { { 1234, newMatchSettingList } };
            var matchSettings2 = new Dictionary<int, List<MatchSetting>> { { 1234, oldMatchSettingList } };

            var newSnapshot = new AssignmentSnapshot
            {
                Matches = matches,
                MatchSettings = matchSettings
            };

            var oldSnapshot = new AssignmentSnapshot
            {
                Matches = matches,
                MatchSettings = matchSettings2
            };

            var snapshot = new ChangedMatchSnapshot { NewMatch = newMatch, OldMatch = oldMatch, NewSnapshot = newSnapshot, OldSnapshot = oldSnapshot };
            return snapshot;
        }

        private ChangedMatchSnapshot DataInputIsNotSpecific10Mins()
        {
            var newMatch = new GeneralMatch
            {
                MatchId = 1234,
                MatchNo = "20000",
                Home = "LiverPool-Coners",
                Away = "Arsenal-Coners",
                LeagueName = "PremierLeague-Coners",
                MatchStatus = "Cancel",
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
                MatchStatus = "Cancel",
                Agent = "Rossi"
            };
            var matches = new Dictionary<int, GeneralMatch> { { 3456, mainMatch } };

            var newMatchSettingList = NewMatchSettingList();
            var oldMatchSettingList = OldMatchSettingList();

            var matchSettings = new Dictionary<int, List<MatchSetting>> { { 1234, newMatchSettingList } };
            var matchSettings2 = new Dictionary<int, List<MatchSetting>> { { 1234, oldMatchSettingList } };

            var newSnapshot = new AssignmentSnapshot
            {
                Matches = matches,
                MatchSettings = matchSettings
            };

            var oldSnapshot = new AssignmentSnapshot
            {
                Matches = matches,
                MatchSettings = matchSettings2
            };

            var snapshot = new ChangedMatchSnapshot { NewMatch = newMatch, OldMatch = oldMatch, NewSnapshot = newSnapshot, OldSnapshot = oldSnapshot };
            return snapshot;
        }

        private List<MatchSetting> OldMatchSettingList()
        {
            var matchSetting = new MatchSetting
            {
                BetType = 1,
                MatchId = 1234,
                MaxBet = 400000,
                Spread = (decimal?)0.5000,
            };

            var matchSetting2 = new MatchSetting
            {
                BetType = 7,
                MatchId = 1234,
                MaxBet = null,
                Spread = (decimal?)0.5000,
            };

            var matchSetting3 = new MatchSetting
            {
                BetType = -1003,
                MatchId = 1234,
                MaxBet = 400000,
                Spread = (decimal?)0.5000,
            };

            var matchSettingList = new List<MatchSetting>
            {
                matchSetting,
                matchSetting2,
                matchSetting3
            };

            return matchSettingList;
        }

        private List<MatchSetting> NewMatchSettingList()
        {
            var matchSetting = new MatchSetting
            {
                BetType = 1,
                MatchId = 1234,
                MaxBet = 300000,
                Spread = (decimal?)0.7000,
            };

            var matchSetting2 = new MatchSetting
            {
                BetType = 7,
                MatchId = 1234,
                MaxBet = 30000,
                Spread = (decimal?)0.5000,
            };

            var matchSetting3 = new MatchSetting
            {
                BetType = -1003,
                MatchId = 1234,
                MaxBet = 400000,
                Spread = (decimal?)0.5000,
            };

            var matchSettingList = new List<MatchSetting>
            {
                matchSetting,
                matchSetting2,
                matchSetting3
            };

            return matchSettingList;
        }
    }
}