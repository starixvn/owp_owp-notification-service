namespace OWP.OtherSportNotification.UnitTests.Services;
public class ChangedAssignmentGeneratorUnitTests
{
    private readonly ChangedAssignmentGeneratorImp services;

    public ChangedAssignmentGeneratorUnitTests()
    {
        var detector = new ChangedDetectorImp();
        services = new ChangedAssignmentGeneratorImp(detector);
    }

    [Fact]
    public void GetDeletedAssignmentChangedValues_Always_ReturnAllDeletedAssignmentChangedValues()
    {
        // Arrange
        var matchSnapshot = GetMatchSnapshot();

        // Act
        var actual = services.GetDeletedAssignmentChangedValues(matchSnapshot).Where(x => x.UserId != 0);

        // Assert
        Assert.Single(actual);
        Assert.Contains(actual, item =>
            item.OSMatchId == 1 &
            item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete &
            item.OldValue == "#starix15" &
            item.NewValue == "#starix1" &
            item.UserId == 15
        );
    }

    [Fact]
    public void GetInsertedAssignmentChangedValues_Always_ReturnAllInsertedAssignmentChangedValues()
    {
        // Arrange
        var matchSnapshot = GetMatchSnapshot();

        // Act
        var actual = services.GetInsertedAssignmentChangedValues(matchSnapshot).Where(x => x.UserId != 0);

        // Assert
        Assert.Single(actual);
        Assert.Contains(actual, item =>
            item.OSMatchId == 1 &
            item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert &
            item.OldValue == "#starix15" &
            item.NewValue == "#starix1" &
            item.UserId == 1
        );
    }

    [Fact]
    public void Create_Always_ReturnAllAssignmentChangedValues()
    {
        // Arrange
        var matchSnapshot = GetMatchSnapshot();
        var existingAssignees = new List<Assignee>
            {
                new() { AssigneeId = 15, AssigneeName = "starix15" },
                new() { AssigneeId = 3, AssigneeName = "starix3" },
                new() { AssigneeId = 4, AssigneeName = "starix4" }
            };

        // Act
        var actual = services.Create(matchSnapshot, existingAssignees);

        // Assert
        Assert.Equal(7, actual.Count);
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert & !c.IsIndirectChangedAssignment));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete & !c.IsIndirectChangedAssignment));
        Assert.Equal(3, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert & c.IsIndirectChangedAssignment));
        Assert.Equal(2, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete & c.IsIndirectChangedAssignment));
        Assert.Contains(actual, item => item.Username == "starix4" & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert);
    }

    private ChangedMatchSnapshot GetMatchSnapshot()
    {
        return new ChangedMatchSnapshot
        {
            OldMatch = GetMatch(1),
            NewMatch = GetMatch(1),
            OldSnapshot = GetSnapshot(15, "starix15", "oldNote"),
            NewSnapshot = GetSnapshot(1, "starix1", "newNote")
        };
    }

    private MatchInfo GetMatch(int matchId)
    {
        return new MatchInfo
        {
            OSMatchId = matchId
        };
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
                    AssignmentNote = "#<user>",
                    UserId = 15,
                    Username = "starix15"
                },
                new()
                {
                    OSMatchId = 1,
                    BetTypeName = "b",
                    AssignmentType = 1,
                    AssignmentNote = "#<user>",
                    UserId = userId,
                    Username = userName
                },
                new()
                {
                    OSMatchId = 1,
                    BetTypeName = "c",
                    AssignmentType = 1,
                    AssignmentNote = "#<user>",
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
                    AssignmentNote = "#<user>",
                    UserId = 4,
                    Username = "starix4"
                }
            };

        var matches = new Dictionary<int, MatchInfo> {
                    { 1, GetMatch(1) },
                    { 10, GetMatch(10) }
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
}
