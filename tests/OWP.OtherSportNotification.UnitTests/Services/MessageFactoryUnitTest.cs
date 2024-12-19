namespace OWP.OtherSportNotification.UnitTests.Services;
public class MessageFactoryUnitTest
{
    private readonly MessageFactory messageFactory = new MessageFactoryImp();

    [Fact]
    public void Transform_Always_ReturnMessageList()
    {
        var changedValueList = GetListChangedValue();

        var expected = new List<MessageModel>
            {
                new()
                {
                   IsImportant = false,
                   MessageContent = "<b>Match No.</b> Updated from 1234 to <b>20000</b> for #20000 - A-Corners - LiverPool-Corners -vs- Arsenal-Corners updated by sa",
                   MessageType = (int)MsgTemplateCode.EditMatchNo,
                   ReceivedUserIds = ["6789"]
                },
                new()
                {
                   IsImportant = false,
                   MessageContent = "<b>Max Bet</b> of <b>Next of Corners (HDP)</b> Updated from 400K to <b>300K</b> for #50000 - PremierLeague - LiverPool -vs- Arsenal updated by sa",
                   MessageType = (int)MsgTemplateCode.EditMaxbet,
                   ReceivedUserIds = ["6789"]
                }
            };

        var actual = messageFactory.Transform(changedValueList, "sa");

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Transform_OtherAssignmentChanged_ReturnMessageList()
    {
        var changedValueList = GetListAssignmentChanged();

        var expected = new List<MessageModel>
            {
                new()
                {
                    IsImportant = true,
                    MessageContent = "You have been REMOVED from <b>1H (HDP)</b> in #1324 - Jo - Spur -vs- MC - 08/08/2021 12:00 AM updated by sa",
                    MessageType = (int)MsgTemplateCode.OtherAssignmentDelete,
                    ReceivedUserIds = ["2542"]
                },
                new()
                {
                    IsImportant = true,
                    MessageContent = "You have NEW ASSIGNMENT <b>1H (HDP)</b> in #5000 - Jo - Spur -vs- MC - 07/08/2021 12:00 AM updated by sa",
                    MessageType = (int)MsgTemplateCode.OtherAssignmentInsert,
                    ReceivedUserIds = ["2542"]
                }
            };

        var actual = messageFactory.Transform(changedValueList, "sa");

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Transform_MatchSettingChanged_ReturnMessageList()
    {
        var changedValueList = GetListMatchSettingChanged();
        var expected = new List<MessageModel>
            {
                new()
                {
                   MessageContent = "<b>Max Bet</b> of <b>FT (HDP)</b> Updated from 12000 to <b>11000</b> for #1234 - League - United -vs- Ha Noi updated by sa",
                   MessageType = (int)MsgTemplateCode.EditMaxbet,
                   ReceivedUserIds = ["8"]
                },
                new()
                {
                   MessageContent = "<b>Max Bet</b> of <b>FT (HDP)</b> Updated from 12000 to <b>11000</b> for #1234 - League - United -vs- Ha Noi updated by sa",
                   MessageType = (int)MsgTemplateCode.EditMaxbet,
                   ReceivedUserIds = ["11"]
                },
                 new()
                 {
                   MessageContent = "<b>Spread</b> of <b>FT (HDP)</b> Updated from 12% to <b>11%</b> for #1234 - League - United -vs- Ha Noi updated by sa",
                   MessageType = (int)MsgTemplateCode.EditSpread,
                   ReceivedUserIds = ["8"]
                 }
            };

        var actual = messageFactory.Transform(changedValueList, "sa");

        DeepAssert.Equal(expected, actual);
    }

    private List<ChangedValue> GetListAssignmentChanged()
    {
        var valueChanged = new ChangedValue
        {
            MessageTypeId = (int)MsgTemplateCode.OtherAssignmentDelete,
            BetTypeName = "1H (HDP)",
            OldValue = "PK Same",
            NewValue = null,
            AssignmentNote = "PK Same",
            AssignmentType = 2,
            OSMatchId = 5567,
            MatchNo = "1324",
            League = "Jo",
            EventName = "Spur -vs- MC",
            UserId = 2542,
            Username = "esther",
            EventDateTime = DateTime.Parse("08/08/2021 12:00 AM")
        };

        var valueChanged2 = new ChangedValue
        {
            MessageTypeId = (int)MsgTemplateCode.OtherAssignmentInsert,
            OldValue = null,
            NewValue = "#esther",
            AssignmentNote = "#<user>",
            AssignmentType = 2,
            BetTypeName = "1H (HDP)",
            OSMatchId = 5567,
            MatchNo = "5000",
            League = "Jo",
            EventName = "Spur -vs- MC",
            UserId = 2542,
            Username = "esther",
            EventDateTime = DateTime.Parse("07/08/2021 12:00 AM")
        };

        var changedValueList = new List<ChangedValue>
            {
                valueChanged,
                valueChanged2
            };

        return changedValueList;
    }

    private List<ChangedValue> GetListChangedValue()
    {
        var valueChanged = new ChangedValue
        {
            MessageTypeId = (int)MsgTemplateCode.EditMatchNo,
            NewValue = "20000",
            OldValue = "1234",
            League = "A-Corners",
            MatchNo = "20000",
            EventName = "LiverPool-Corners -vs- Arsenal-Corners",
            UserId = 6789,
            Username = "esther"
        };
        var valueChanged2 = new ChangedValue
        {
            OldValue = "400K",
            NewValue = "300K",
            BetTypeName = "Next of Corners (HDP)",
            MatchNo = "50000",
            League = "PremierLeague",
            EventName = "LiverPool -vs- Arsenal",
            UserId = 6789,
            Username = "esther",
            MessageTypeId = (int)MsgTemplateCode.EditMaxbet
        };

        var changedValueList = new List<ChangedValue>
            {
                valueChanged,
                valueChanged2
            };

        return changedValueList;
    }

    private List<ChangedValue> GetListMatchSettingChanged()
    =>
    [
        new()
        {
            UserId = 8,
            Username = "Liam",
            BetTypeName = "FT (HDP)",
            EventName = "United -vs- Ha Noi",
            League = "League",
            MatchNo = "1234",
            MessageTypeId = (int)MsgTemplateCode.EditMaxbet,
            NewValue = "11000",
            OldValue = "12000"
        },

        new()
        {
            UserId = 11,
            Username = "Starix1",
            BetTypeName = "FT (HDP)",
            EventName = "United -vs- Ha Noi",
            League = "League",
            MatchNo = "1234",
            MessageTypeId = (int)MsgTemplateCode.EditMaxbet,
            NewValue = "11000",
            OldValue = "12000"
        },

        new()
        {
            UserId = 8,
            Username = "Liam",
            BetTypeName = "FT (HDP)",
            EventName = "United -vs- Ha Noi",
            League = "League",
            MatchNo = "1234",
            MessageTypeId = (int)MsgTemplateCode.EditSpread,
            NewValue = "11%",
            OldValue = "12%"
        }
    ];
}
