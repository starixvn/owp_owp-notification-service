using OWP.AssignmentUploadNotification.Constants;
using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

public class ChangedMatchChangedValueGeneratorUnitTests
{
    [Fact]
    public void GetAllIndirectAssignees_MainMatch_ReturnAllExistingAssigneesContainMinormatch()
    {
        // Arrange
        var oldSnapshot = GetSnapshot(15, "starix15", "oldNote");
        var newSnapshot = GetSnapshot(1, "starix1", "newNote");
        var expected = new List<Assignee>
        {
            new() { AssigneeId = 15, AssigneeName = "starix15" },
            new() { AssigneeId = 3, AssigneeName = "starix3" },
            new() { AssigneeId = 4, AssigneeName = "starix4" }
        };

        // Act
        var actual = ChangedMatchChangedValueGeneratorImp.GetAllIndirectAssignees(1, oldSnapshot, newSnapshot);

        // Assert
        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void GetAllIndirectAssignees_MinorMatch_ReturnAllExistingAssigneesContainMainMatch()
    {
        // Arrange
        var oldSnapshot = GetSnapshot(15, "starix15", "oldNote");
        var newSnapshot = GetSnapshot(1, "starix1", "newNote");
        var expected = new List<Assignee>
        {
            new() { AssigneeId = 4, AssigneeName = "starix4" },
            new() { AssigneeId = 15, AssigneeName = "starix15" },
            new() { AssigneeId = 3, AssigneeName = "starix3" }
        };

        // Act
        var actual = ChangedMatchChangedValueGeneratorImp.GetAllIndirectAssignees(10, oldSnapshot, newSnapshot);

        // Assert
        DeepAssert.Equal(expected, actual);
    }

    private AssignmentSnapshot GetSnapshot(int userId, string userName, string note)
    {
        var assignmentList = new List<MatchAssignment> {
            new()
            {
                MatchId = 1,
                BetType = 1,
                AssignmentType = 2,
                AssignmentNote = note,
                UserId = 0,
                UserName = null
            },
            new()
            {
                MatchId = 1,
                BetType = 1,
                AssignmentType = 1,
                AssignmentNote = null,
                UserId = 15,
                UserName = "starix15"
            },
            new()
            {
                MatchId = 1,
                BetType = 3,
                AssignmentType = 1,
                AssignmentNote = null,
                UserId = userId,
                UserName = userName
            },
            new()
            {
                MatchId = 1,
                BetType = 7,
                AssignmentType = 1,
                AssignmentNote = null,
                UserId = 3,
                UserName = "starix3"
            }
        };
        var assignmentList2 = new List<MatchAssignment> {
            new()
            {
                MatchId = 10,
                BetType = (int)MinorBettype.Specific10Mins,
                AssignmentType = 1,
                AssignmentNote = null,
                UserId = 4,
                UserName = "starix4"
            }
        };

        var matches = new Dictionary<int, GeneralMatch> {
            { 1, GetMatch(1, 0, 1) },
            { 10, GetMatch(10, 1, -5) },
            { 2, GetMatch(10, 1, -3) }
        };

        return new AssignmentSnapshot
        {
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1, assignmentList },
                { 10, assignmentList2 }
            },
            Matches = matches
        };
    }

    private GeneralMatch GetMatch(int matchId, int parentMatchId, int matchType)
    {
        return new GeneralMatch
        {
            MatchId = matchId,
            ParentMatchId = parentMatchId,
            MatchType = matchType,
            MinorBettypeName = "name"
        };
    }
}