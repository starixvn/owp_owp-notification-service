namespace OWP.OtherSportNotification.UnitTests.Services;
public class ChangedMatchStatusGeneratorUnitTests
{
    [Fact]
    public void BuildListMatchChangeMatchStatus_Snapshot_Has_NewMatch_OldMatch_Diff_MatchStatus()
    {
        var changedMatchStatusGeneratorImp = new ChangedMatchStatusGeneratorImp();
        var snapshot = new ChangedMatchSnapshot
        {
            OldMatch = new MatchInfo
            {
                MatchNo = 123,
                OSMatchId = 123,
                OfferLive = "CANCEL",
                League = "League",
                Home = "United",
                Away = "Ha Noi"
            },
            NewMatch = new MatchInfo
            {
                MatchNo = 123,
                OSMatchId = 123,
                OfferLive = "LIVE",
                League = "League",
                Home = "United",
                Away = "Ha Noi"
            }
        };
        var assignees = new List<Assignee>
            {
                new()
                {
                    AssigneeId=8,
                    AssigneeName="sa"
                }
            };
        var actual = changedMatchStatusGeneratorImp.Create(snapshot, assignees);
        var expected = new List<ChangedValue>
            {
                new()
                {
                    UserId=8,
                    Username="sa",
                    EventName="United -vs- Ha Noi",
                    League="League",
                    MatchNo="123",
                    MessageTypeId=108,
                    NewValue="LIVE",
                    OldValue="CANCEL"
                }
            };
        DeepAssert.Equal(expected, actual);
    }
}
