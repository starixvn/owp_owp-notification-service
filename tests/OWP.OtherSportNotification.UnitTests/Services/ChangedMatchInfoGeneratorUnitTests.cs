namespace OWP.OtherSportNotification.UnitTests.Services;
public class ChangedMatchInfoGeneratorUnitTests
{
    [Fact]
    public void BuildListMatchChanged_MatchHasValueChange_ReturnListChangedValue()
    {
        var changedMatchInfoGeneratorImp = new ChangedMatchInfoGeneratorImp();
        var oldMatch = new MatchInfo
        {
            MatchNo = 123,
            Category = "AB2",
            MatchOffer = "offer",
            AutoLink = "Yes",
            OtherProvider = "Other1",
            IPTV = "TV1",
            WB = "Streaming1",
            Remark = "remark1",
            Reporter = "Reporter1",
            Provider = "Provider1"
        };
        var newMatch = new MatchInfo
        {
            MatchNo = 123,
            Category = "AB22",
            MatchOffer = "offer1",
            AutoLink = "No",
            OtherProvider = "Other2",
            IPTV = "TV2",
            WB = "Streaming2",
            Remark = "remark2",
            Reporter = "Reporter2",
            Provider = "Provider2"
        };
        var actual = changedMatchInfoGeneratorImp.GetListValueChanged(oldMatch, newMatch);
        var expected = new List<ChangedValue>
            {
                new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditCategory,
                    NewValue="AB22",
                    OldValue="AB2"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditMatchOffer,
                    NewValue="offer1",
                    OldValue="offer"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditAutoLink,
                    NewValue="No",
                    OldValue="Yes"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditOtherProvider,
                    NewValue="Other2",
                    OldValue="Other1"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditIPTV,
                    NewValue="TV2",
                    OldValue="TV1"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditWB,
                    NewValue="Streaming2",
                    OldValue="Streaming1"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditRemark,
                    NewValue="remark2",
                    OldValue="remark1"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditReporter,
                    OldValue="Reporter1",
                    NewValue="Reporter2"
                },new ChangedValue
                {
                    MessageTypeId=(int)MsgTemplateCode.EditProvider,
                    OldValue="Provider1",
                    NewValue="Provider2"
                },
            };
        DeepAssert.Equals(expected, actual);
    }

    [Fact]
    public static void CreateListMatchInfoChange_ListMatchHasInfoChange()
    {
        var changedMatchInfoGeneratorImp = new ChangedMatchInfoGeneratorImp();
        var oldMatch = new MatchInfo
        {
            MatchNo = 123,
            Category = "AB2",
            MatchOffer = "offer",
            AutoLink = "Yes",
            OtherProvider = "Other1",
            IPTV = "TV1",
            WB = "Streaming1",
            Remark = "remark1",
            Reporter = "Reporter1",
            Provider = "Provider1",
            League = "VLeague",
            Home = "Binh Dinh",
            Away = "Tphcm"
        };
        var newMatch = new MatchInfo
        {
            MatchNo = 123,
            Category = "AB22",
            MatchOffer = "offer1",
            AutoLink = "No",
            OtherProvider = "Other2",
            IPTV = "TV2",
            WB = "Streaming2",
            Remark = "remark2",
            Reporter = "Reporter2",
            Provider = "Provider2",
            League = "VLeague",
            Home = "Binh Dinh",
            Away = "Tphcm"
        };

        var assignees = new List<Assignee>
            {
                new Assignee
                {
                    AssigneeId=8,
                    AssigneeName="sa"
                }
            };
        var expected = new List<ChangedValue>
            {
                new ChangedValue
                {
                    MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditCategory,
                    NewValue="AB22",
                    OldValue="AB2",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditMatchOffer,
                    NewValue="offer1",
                    OldValue="offer",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditAutoLink,
                    NewValue="No",
                    OldValue="Yes",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditOtherProvider,
                    NewValue="Other2",
                    OldValue="Other1",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditIPTV,
                    NewValue="TV2",
                    OldValue="TV1",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditWB,
                    NewValue="Streaming2",
                    OldValue="Streaming1",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditRemark,
                    NewValue="remark2",
                    OldValue="remark1",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditReporter,
                    OldValue="Reporter1",
                    NewValue="Reporter2",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },new ChangedValue
                {
                     MatchNo = "123",
                    MessageTypeId=(int)MsgTemplateCode.EditProvider,
                    OldValue="Provider1",
                    NewValue="Provider2",
                    UserId=8,
                    Username="sa",
                    League="VLeague",
                    EventName= "Binh Dinh -vs- Tphcm"
                },
            };
        var actual = changedMatchInfoGeneratorImp.Create(oldMatch, newMatch, assignees);
        DeepAssert.Equal(expected, actual);
    }
}
