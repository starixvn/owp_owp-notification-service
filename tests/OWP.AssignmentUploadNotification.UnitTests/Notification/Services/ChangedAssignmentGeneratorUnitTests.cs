using OWP.AssignmentUploadNotification.Constants;
using OWP.AssignmentUploadNotification.Models;
using OWP.AssignmentUploadNotification.Services;
using Xunit;

namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Services;

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
        var actual = services.GetDeletedAssignmentChangedValues(matchSnapshot).Where(x => x.AssigneeId != 0);

        // Assert
        Assert.Single(actual);
        Assert.Contains(actual, item =>
            item.MatchId == 1 &
            item.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete &
            item.OldValue == "#starix15" &
            item.NewValue == null &
            item.AssigneeId == 15
        );
    }

    [Fact]
    public void GetInsertedAssignmentChangedValues_Always_ReturnAllInsertedAssignmentChangedValues()
    {
        // Arrange
        var matchSnapshot = GetMatchSnapshot();

        // Act
        var actual = services.GetInsertedAssignmentChangedValues(matchSnapshot).Where(x => x.AssigneeId != 0);

        // Assert
        Assert.Single(actual);
        Assert.Contains(actual, item =>
            item.MatchId == 1 &
            item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert &
            item.OldValue == null &
            item.NewValue == "#starix1" &
            item.AssigneeId == 1
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
        Assert.Equal(14, actual.Count);
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert & c.IsIndirectChangedAssignment == false));
        Assert.Equal(1, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete & c.IsIndirectChangedAssignment == false));
        Assert.Equal(3, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert & c.IsIndirectChangedAssignment));
        Assert.Equal(3, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete & c.IsIndirectChangedAssignment));
        Assert.Equal(3, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentInsert & c.IsIndirectChangedAssignment));
        Assert.Equal(3, actual.Count(c => c.MessageTypeId == (int)MsgTemplateCode.OtherAssignmentDelete & c.IsIndirectChangedAssignment));
        Assert.Contains(actual, item => item.AssigneeName == "starix4" & item.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert);
    }

    [Fact]
    public void GetAssignmentsOfMatchWhichIsChangedToLive_Always_ReturnEmpty()
    {
        // Arrange
        var matchSnapshot = GetMatchSnapshot("POSTPONED", "Live");

        // Act
        var actual = ChangedAssignmentGeneratorImp.GetAssignmentsOfMatchWhichIsChangedToLive(matchSnapshot);

        // Assert
        Assert.Empty(actual);
    }

    private ChangedMatchSnapshot GetMatchSnapshot(string oldStatus = "Live", string newStatus = "Live")
    {
        return new ChangedMatchSnapshot
        {
            OldMatch = GetMatch(1, 0, oldStatus),
            NewMatch = GetMatch(1, 0, newStatus),
            OldSnapshot = GetSnapshot(15, "starix15", "oldNote"),
            NewSnapshot = GetSnapshot(1, "starix1", "newNote")
        };
    }

    private GeneralMatch GetMatch(int matchId, int parentMatchId, string matchStatus = "Live")
    {
        return new GeneralMatch
        {
            MatchId = matchId,
            ParentMatchId = parentMatchId,
            MatchStatus = matchStatus,
            MatchType = 1,
            MinorBettypeName = "name"
        };
    }

    private AssignmentSnapshot GetSnapshot(int userId, string userName, string note, string matchStatus = "Live")
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
            { 1, GetMatch(1, 0, matchStatus) },
            { 10, GetMatch(10, 1, matchStatus) }
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
}