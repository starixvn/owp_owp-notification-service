namespace OWP.PreMatchNotification.UnitTests.Services;

public class MessageFactoryUnitTest
{
    private static MessageFactory MockService()
    {
        return new MessageFactoryImp();
    }

    [Fact]
    public void Transform_WithAddNewLeague_ReturnMessageList()
    {
        var service = MockService();
        var changedValueList = GetListNewLeague();
        var expected = new List<MessageModel>
        {
            new()
            {
               IsBlinking=true,
               IsImportant = true,
               MessageContent = "New Add League",
               MessageType = (int)MsgTemplateCode.NewLeague,
               ReceivedUserIds = ["2", "3", "5"]
            }
        };

        var actual = service.Transform(changedValueList);

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Transform_WithAssignmentChanged_ReturnMessageList()
    {
        var service = MockService();
        var changedValueList = GetListAssignmentChanged();
        var expected = new List<MessageModel>
        {
            new()
            {
               IsImportant = true,
               MessageContent = "[DayShift] You have NEW ASSIGNMENT <b>O/U</b> in <b>League</b>",
               MessageType = (int)MsgTemplateCode.DayShiftAssignmentInsert,
               ReceivedUserIds = ["12", "1"]
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[DayShift] You have been REMOVED from <b>O/U</b> in <b>League</b>",
               MessageType = (int)MsgTemplateCode.DayShiftAssignmentDelete,
               ReceivedUserIds = ["2"]
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[DayShift] O/U's Assignee has been updated from <b>Sa</b> to <b>Starix15</b> in your assignment <b>League</b>",
               MessageType = (int)MsgTemplateCode.DayShiftOtherAssignmentUpdate,
               ReceivedUserIds = ["13"]
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[NightShift] You have NEW ASSIGNMENT <b>O/U</b> in <b>ThaiLeague</b>",
               MessageType = (int)MsgTemplateCode.NightShiftAssignmentInsert,
               ReceivedUserIds = ["1"]
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[NightShift] You have been REMOVED from <b>O/U</b> in <b>ThaiLeague</b>",
               MessageType = (int)MsgTemplateCode.NightShiftAssignmentDelete,
               ReceivedUserIds = ["12"]
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[NightShift] O/U's Assignee has been updated from <b>Sa</b> to <b>Starix15</b> in your assignment <b>ThaiLeague</b>",
               MessageType = (int)MsgTemplateCode.NightShiftOtherAssignmentUpdate,
               ReceivedUserIds = ["13"]
            }
        };

        var actual = service.Transform(changedValueList);

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Transform_WithLinkedMatchChanged_ReturnMessageList()
    {
        var service = MockService();
        var changedValueList = GetListLinkedMatchChanged();
        var expected = new List<MessageModel>
        {
            new()
            {
               IsBlinking=true,
               IsImportant = true,
               MessageContent = "Has been <b>Live</b> for match: LeagueA - HomeA -vs- AwayA; Previous Postponed",
               MessageType = (int)MsgTemplateCode.LGAChangeMatchStatus,
               ReceivedUserIds = ["1", "3", "8"]
            },
            new()
            {
               IsBlinking=true,
               IsImportant = true,
               MessageContent = "ADD LIVE for 05:12 AM - LeagueC - HomeA -vs- AwayA",
               MessageType = (int)MsgTemplateCode.LGAAddLiveMatch,
               ReceivedUserIds = ["1", "2", "3"]
            },
            new()
            {
               IsBlinking=true,
               IsImportant = true,
               MessageContent = "CANCEL LIVE for 04:02 AM - League - HomeA -vs- AwayA",
               MessageType = (int)MsgTemplateCode.LGACancelLiveMatch,
               ReceivedUserIds = ["1", "2"]
            }
        };

        var actual = service.Transform(changedValueList);

        DeepAssert.Equal(expected, actual);
    }

    private IList<ChangedValue> GetListNewLeague()
    {
        return new List<ChangedValue>
        {
            new()
            {
                League = "League",
                ReceivedUserIds = [2, 3],
                MessageTypeId= (int) MsgTemplateCode.NewLeague
            },
            new()
            {
                League = "League",
                ReceivedUserIds = [3, 5],
                MessageTypeId= (int) MsgTemplateCode.NewLeague
            }
        };
    }

    private IList<ChangedValue> GetListAssignmentChanged()
    {
        return new List<ChangedValue>
        {
            new()
            {
                BetTypeName = "O/U",
                League = "League",
                MessageTypeId = (int)MsgTemplateCode.DayShiftAssignmentInsert,
                ReceivedUserIds = [12, 1]
            },
            new()
            {
                BetTypeName = "O/U",
                League = "League",
                MessageTypeId = (int)MsgTemplateCode.DayShiftAssignmentDelete,
                ReceivedUserIds = [2]
            },
            new()
            {
                League = "League",
                BetTypeName = "O/U",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = [13],
                MessageTypeId = (int)MsgTemplateCode.DayShiftOtherAssignmentUpdate
            },
            new()
            {
                BetTypeName = "O/U",
                League = "ThaiLeague",
                MessageTypeId = (int)MsgTemplateCode.NightShiftAssignmentInsert,
                ReceivedUserIds = [1]
            },
            new()
            {
                BetTypeName = "O/U",
                League = "ThaiLeague",
                MessageTypeId = (int)MsgTemplateCode.NightShiftAssignmentDelete,
                ReceivedUserIds = [12]
            },
            new()
            {
                League = "ThaiLeague",
                BetTypeName = "O/U",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = [13],
                MessageTypeId = (int)MsgTemplateCode.NightShiftOtherAssignmentUpdate
            }
        };
    }

    private IList<ChangedValue> GetListLinkedMatchChanged()
    {
        return new List<ChangedValue>
        {
            new()
            {
                MessageTypeId=(int)MsgTemplateCode.LGAChangeMatchStatus,
                League="LeagueA",
                Home="HomeA",
                Away="AwayA",
                OldValue = "Postponed",
                NewValue = "Live",
                ReceivedUserIds= [1, 3, 8]
            },
            new()
            {
                MessageTypeId=(int)MsgTemplateCode.LGAAddLiveMatch,
                League="LeagueC",
                Home="HomeA",
                Away="AwayA",
                KoTime = "05:12 AM",
                ReceivedUserIds= [1, 2, 3]
            },
            new()
            {
                MessageTypeId=(int)MsgTemplateCode.LGACancelLiveMatch,
                League="League",
                Home="HomeA",
                Away="AwayA",
                KoTime = "04:02 AM",
                ReceivedUserIds= [1, 2]
            }
        };
    }
}