using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

public class ChangedMatchStatusGeneratorUnitTest
{

    [Fact]

    public void Create_MinorMatchStatusIsCancel_ReturnChangedValue()
    {
        var snapshot = GetMinorMatchSnapshot();
        var assignments = new List<Assignee>
        {
            new()
            {
                AssigneeId = 6789,
                AssigneeName = "esther"
            }
        };

        var expected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 7,
                OldValue = "Live",
                NewValue = "Cancel",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                BetTypeName = "Next of Corners"
            }
        };

        var changedMatchStatus = new ChangedMatchStatusGeneratorImp();
        var actual = changedMatchStatus.Create(snapshot, assignments);

        // Assert
        DeepAssert.Equal(expected, actual);
    }

    [Fact]

    public void Create_MainMatchStatusIsCancel_ReturnChangedValue()
    {
        var snapshot = GetMainMatchSnapshot();
        var assignments = new List<Assignee>
        {
            new()
            {
                AssigneeId = 6789,
                AssigneeName = "esther"
            }
        };

        var expected = new List<ChangedValue>
        {
            new()
            {
                MessageTypeId = 6,
                OldValue = "Live",
                NewValue = "Cancel",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther"
            }
        };

        var changedMatchStatus = new ChangedMatchStatusGeneratorImp();
        var actual = changedMatchStatus.Create(snapshot, assignments);

        // Assert
        DeepAssert.Equal(expected, actual);
    }

    [Fact]

    public void Create__IsIgnoredMinorMatch_ReturnNewChangedValue()
    {
        var snapshot = MinorMatchSnapshot();
        var assignments = new List<Assignee>
        {
            new()
            {
                AssigneeId = 6789,
                AssigneeName = "esther"
            }
        };

        var expected = new List<ChangedValue>();
        var changedMatchStatus = new ChangedMatchStatusGeneratorImp();

        var actual = changedMatchStatus.Create(snapshot, assignments);

        // Assert
        DeepAssert.Equal(expected, actual);
    }

    [Fact]

    public void Create_NewMatchStatusIsNotCancelSuspendedPostponed_ReturnNewChangedValue()
    {
        var snapshot = SnapshotData();
        var assignments = new List<Assignee>
        {
            new()
            {
                AssigneeId = 6789,
                AssigneeName = "esther"
            }
        };

        var expected = new List<ChangedValue>();

        var changedMatchStatus = new ChangedMatchStatusGeneratorImp();
        var actual = changedMatchStatus.Create(snapshot, assignments);

        // Assert
        DeepAssert.Equal(expected, actual);
    }

    private ChangedMatchSnapshot GetMinorMatchSnapshot()
    {
        var minorNewMatch = new GeneralMatch
        {
            MatchId = 1234,
            MatchNo = "20000",
            Home = "LiverPool-Corners",
            Away = "Arsenal-Corners",
            LeagueName = "PremierLeague-Corners",
            MatchStatus = "Cancel",
            Agent = "Rose",
            ParentMatchId = 3456,
            MatchType = 1000,
            MinorBettypeName = "Next of Corners",
            Division = "20",
            Remark = "check",
            WB = ""

        };

        var minorOldMatch = new GeneralMatch
        {
            MatchId = 1234,
            MatchNo = "1234",
            Home = "LiverPool-Corners",
            Away = "Arsenal-Corners",
            LeagueName = "PremierLeague-Corners",
            MatchStatus = "Live",
            Agent = "Bow",
            ParentMatchId = 3456,
            MatchType = 1000,
            MinorBettypeName = "Next of Corners",
            Division = "10",
            Remark = "",
            WB = "WB@"
        };

        var newMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Cancel",
            Agent = "Rose"

        };

        var oldMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Live",
            Agent = "Rose"

        };
        var newMatches = new Dictionary<int, GeneralMatch> { { 3456, newMainMatch } };

        var oldMatches = new Dictionary<int, GeneralMatch> { { 3456, oldMainMatch } };

        var newSnapshot = new AssignmentSnapshot
        {
            Matches = newMatches
        };

        var oldSnapshot = new AssignmentSnapshot
        {
            Matches = oldMatches
        };

        var snapshot = new ChangedMatchSnapshot { NewMatch = minorNewMatch, OldMatch = minorOldMatch, NewSnapshot = newSnapshot, OldSnapshot = oldSnapshot };
        return snapshot;
    }

    private ChangedMatchSnapshot GetMainMatchSnapshot()
    {
        var newMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Cancel",
            Agent = "Rose"

        };

        var oldMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Live",
            Agent = "Rose"

        };

        var snapshot = new ChangedMatchSnapshot { NewMatch = newMainMatch, OldMatch = oldMainMatch };
        return snapshot;
    }

    private ChangedMatchSnapshot SnapshotData()
    {
        var newMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Live",
            Agent = "Rose"

        };

        var oldMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Cancel",
            Agent = "Rose"

        };

        var snapshot = new ChangedMatchSnapshot { NewMatch = newMainMatch, OldMatch = oldMainMatch };
        return snapshot;
    }

    private ChangedMatchSnapshot MinorMatchSnapshot()
    {
        var minorNewMatch = new GeneralMatch
        {
            MatchId = 1234,
            MatchNo = "20000",
            Home = "LiverPool-Corners",
            Away = "Arsenal-Corners",
            LeagueName = "PremierLeague-FAST MARKET",
            MatchStatus = "Cancel",
            Agent = "Rose",
            ParentMatchId = 3456,
            MatchType = -88,
            MinorBettypeName = "Fast Market",
            Division = "20",
            Remark = "check",
            WB = ""

        };

        var minorOldMatch = new GeneralMatch
        {
            MatchId = 1234,
            MatchNo = "1234",
            Home = "LiverPool-Corners",
            Away = "Arsenal-Corners",
            LeagueName = "PremierLeague-FAST MARKET",
            MatchStatus = "Live",
            Agent = "Bow",
            ParentMatchId = 3456,
            MatchType = -88,
            MinorBettypeName = "Fast Market",
            Division = "10",
            Remark = "",
            WB = "WB@"
        };

        var newMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Cancel",
            Agent = "Rose"

        };

        var oldMainMatch = new GeneralMatch
        {
            MatchId = 3456,
            MatchNo = "50000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague",
            MatchStatus = "Live",
            Agent = "Rose"

        };
        var newMatches = new Dictionary<int, GeneralMatch> { { 3456, newMainMatch } };

        var oldMatches = new Dictionary<int, GeneralMatch> { { 3456, oldMainMatch } };

        var newSnapshot = new AssignmentSnapshot
        {
            Matches = newMatches
        };

        var oldSnapshot = new AssignmentSnapshot
        {
            Matches = oldMatches
        };

        var snapshot = new ChangedMatchSnapshot { NewMatch = minorNewMatch, OldMatch = minorOldMatch, NewSnapshot = newSnapshot, OldSnapshot = oldSnapshot };
        return snapshot;
    }
}