namespace OWP.OtherSportNotification.UnitTests.Services;
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
                new() { AssigneeId = 3, AssigneeName = "starix3" }
            };

        // Act
        var actual = ChangedMatchChangedValueGeneratorImp.GetAllIndirectAssignees(1, oldSnapshot, newSnapshot);

        // Assert
        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void GetAllIndirectAssignees_ReturnAllExistingAssigneesContainMainmatch()
    {
        // Arrange
        var oldSnapshot = GetSnapshot(15, "starix15", "oldNote");
        var newSnapshot = GetSnapshot(1, "starix1", "newNote");
        var expected = new List<Assignee>
            {
                new() { AssigneeId = 4, AssigneeName = "starix4" }
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
                    OSMatchId = 1,
                    BetTypeName = "a",
                    AssignmentType = 2,
                    AssignmentNote = note,
                    UserId = 0,
                    Username = null
                },
                new()
                {
                    OSMatchId = 1,
                    BetTypeName = "a",
                    AssignmentType = 1,
                    AssignmentNote = null,
                    UserId = 15,
                    Username = "starix15"
                },
                new()
                {
                    OSMatchId = 1,
                    BetTypeName = "b",
                    AssignmentType = 1,
                    AssignmentNote = null,
                    UserId = userId,
                    Username = userName
                },
                new()
                {
                    OSMatchId = 1,
                    BetTypeName = "c",
                    AssignmentType = 1,
                    AssignmentNote = null,
                    UserId = 3,
                    Username = "starix3"
                }
            };
        var assignmentList2 = new List<MatchAssignment> {
                new()
                {
                    OSMatchId = 10,
                    BetTypeName = "d",
                    AssignmentType = 1,
                    AssignmentNote = null,
                    UserId = 4,
                    Username = "starix4"
                }
            };

        var matches = new Dictionary<int, MatchInfo> {
                    { 1, GetMatch(1) },
                    { 10, GetMatch(10) },
                    { 2, GetMatch(10) }
                };

        return new AssignmentSnapshot
        {
            MatchAssignments = new Dictionary<int, List<MatchAssignment>>
                {
                    { 1, assignmentList },
                    { 10, assignmentList2 }
                },
            Matches = matches
        };
    }

    private MatchInfo GetMatch(int matchId)
    {
        return new MatchInfo
        {
            OSMatchId = matchId
        };
    }
}
