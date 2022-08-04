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
                League = "Vleague",
                Home = "Binh Dinh",
                Away = "Ha Noi"
            },
            NewMatch = new MatchInfo
            {
                MatchNo = 123,
                OSMatchId = 123,
                OfferLive = "LIVE",
                League = "Vleague",
                Home = "Binh Dinh",
                Away = "Ha Noi"
            },
        };
        var assignees = new List<Assignee>
            {
                new Assignee
                {
                    AssigneeId=8,
                    AssigneeName="sa"
                }
            };
        var actual = changedMatchStatusGeneratorImp.Create(snapshot, assignees);
        var expected = new List<ChangedValue>
            {
                new ChangedValue
                {
                    UserId=8,
                    Username="sa",
                    EventName="Binh Dinh -vs- Ha Noi",
                    League="Vleague",
                    MatchNo="1234",
                    MessageTypeId=10,
                    NewValue="LIVE",
                    OldValue="CANCEL",
                }
            };
        DeepAssert.Equals(expected, actual);
    }
}
