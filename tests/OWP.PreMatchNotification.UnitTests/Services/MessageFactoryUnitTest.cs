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
               ReceivedUserIds = new List<string> { "2","3","5"}
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
               ReceivedUserIds = new List<string> { "12","1"}
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[DayShift] You have been REMOVED from <b>O/U</b> in <b>League</b>",
               MessageType = (int)MsgTemplateCode.DayShiftAssignmentDelete,
               ReceivedUserIds = new List<string> { "2"}
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[DayShift] O/U's Assignee has been updated from <b>Sa</b> to <b>Starix15</b> in your assignment <b>League</b>",
               MessageType = (int)MsgTemplateCode.DayShiftOtherAssignmentUpdate,
               ReceivedUserIds = new List<string> { "13"}
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[NightShift] You have NEW ASSIGNMENT <b>O/U</b> in <b>ThaiLeague</b>",
               MessageType = (int)MsgTemplateCode.NightShiftAssignmentInsert,
               ReceivedUserIds = new List<string> {"1"}
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[NightShift] You have been REMOVED from <b>O/U</b> in <b>ThaiLeague</b>",
               MessageType = (int)MsgTemplateCode.NightShiftAssignmentDelete,
               ReceivedUserIds = new List<string> { "12"}
            },
            new()
            {
               IsImportant = true,
               MessageContent = "[NightShift] O/U's Assignee has been updated from <b>Sa</b> to <b>Starix15</b> in your assignment <b>ThaiLeague</b>",
               MessageType = (int)MsgTemplateCode.NightShiftOtherAssignmentUpdate,
               ReceivedUserIds = new List<string> { "13"}
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
               ReceivedUserIds = new List<string> { "1","3","8"}
            },
            new()
            {
               IsBlinking=true,
               IsImportant = true,
               MessageContent = "ADD LIVE for 05:12 AM - LeagueC - HomeA -vs- AwayA",
               MessageType = (int)MsgTemplateCode.LGAAddLiveMatch,
               ReceivedUserIds = new List<string> { "1","2","3"}
            },
            new()
            {
               IsBlinking=true,
               IsImportant = true,
               MessageContent = "CANCEL LIVE for 04:02 AM - League - HomeA -vs- AwayA",
               MessageType = (int)MsgTemplateCode.LGACancelLiveMatch,
               ReceivedUserIds = new List<string> { "1","2"}
            }
        };

        var actual = service.Transform(changedValueList);

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Transform_WithMatchInfoChanged_ReturnMessageList()
    {
        var service = MockService();
        var changedValueList = GetListMatchInfoChanged();
        var expected = new List<MessageModel>
        {
            new()
            {
                IsImportant = true,
                MessageContent = "<b>Home Team</b> Update from Premier League - <b>MC</b> -vs- MU to Premier League - <b>MC changed</b> -vs- MU",
                MessageType = (int)MsgTemplateCode.UpdateHome,
                ReceivedUserIds = new List<string> { "2","3","4","5","6","7","8"}
            },
            new()
            {
                IsImportant = true,
                MessageContent = "<b>Away Team</b> Update from Premier League - Liv -vs- <b>MC</b> to Premier League - Liv -vs- <b>MC changed</b>",
                MessageType = (int)MsgTemplateCode.UpdateAway,
                ReceivedUserIds = new List<string> { "2","3","4","5","6"}
            },
            new()
            {
                IsImportant = true,
                MessageContent = "<b>K.O time</b> Update from <b>06/23/2021 03:00 PM</b> to <b>09/03/2022 03:00 PM</b> for Premier League - Liv -vs- MC",
                MessageType = (int)MsgTemplateCode.UpdateKOTime,
                ReceivedUserIds = new List<string> { "2","3"}
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
                ReceivedUserIds = new List<int> { 2, 3 },
                MessageTypeId= (int) MsgTemplateCode.NewLeague
            },
            new()
            {
                League = "League",
                ReceivedUserIds = new List<int> { 3, 5 },
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
                ReceivedUserIds = new List<int>{12,1}
            },
            new()
            {
                BetTypeName = "O/U",
                League = "League",
                MessageTypeId = (int)MsgTemplateCode.DayShiftAssignmentDelete,
                ReceivedUserIds = new List<int>{2}
            },
            new()
            {
                League = "League",
                BetTypeName = "O/U",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = new List<int>{13},
                MessageTypeId = (int)MsgTemplateCode.DayShiftOtherAssignmentUpdate
            },
            new()
            {
                BetTypeName = "O/U",
                League = "ThaiLeague",
                MessageTypeId = (int)MsgTemplateCode.NightShiftAssignmentInsert,
                ReceivedUserIds = new List<int>{1}
            },
            new()
            {
                BetTypeName = "O/U",
                League = "ThaiLeague",
                MessageTypeId = (int)MsgTemplateCode.NightShiftAssignmentDelete,
                ReceivedUserIds = new List<int>{12}
            },
            new()
            {
                League = "ThaiLeague",
                BetTypeName = "O/U",
                OldValue = "Sa",
                NewValue = "Starix15",
                ReceivedUserIds = new List<int>{13},
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
                ReceivedUserIds=new List<int>{1,3,8}
            },
            new()
            {
                MessageTypeId=(int)MsgTemplateCode.LGAAddLiveMatch,
                League="LeagueC",
                Home="HomeA",
                Away="AwayA",
                KoTime = "05:12 AM",
                ReceivedUserIds=new List<int>{1,2,3}
            },
            new()
            {
                MessageTypeId=(int)MsgTemplateCode.LGACancelLiveMatch,
                League="League",
                Home="HomeA",
                Away="AwayA",
                KoTime = "04:02 AM",
                ReceivedUserIds=new List<int>{1,2}
            }
        };
    }

    private IList<ChangedValue> GetListMatchInfoChanged()
    {
        return new List<ChangedValue>
        {
            new()
            {
                InfoSiteMatchId = 1,
                League="Premier League",
                Home = "MC",
                Away = "MU",
                OldValue = "MC" ,
                NewValue =  "MC changed",
                ReceivedUserIds = new List<int> { 2, 2, 3, 4, 5, 6 },
                MessageTypeId = (int)MsgTemplateCode.UpdateHome
            },
            new()
            {
                InfoSiteMatchId = 7,
                League="Premier League",
                Home = "MC",
                Away = "MU",
                OldValue = "MC" ,
                NewValue =  "MC changed",
                ReceivedUserIds = new List<int> { 5, 6 ,7, 8 },
                MessageTypeId = (int)MsgTemplateCode.UpdateHome
            },
            new()
            {
                InfoSiteMatchId = 2,
                League = "Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue = "MC",
                NewValue = "MC changed",
                ReceivedUserIds = new List<int> { 2, 3, 4, 5, 6 },
                MessageTypeId = (int)MsgTemplateCode.UpdateAway
            },
            new()
            {
                InfoSiteMatchId = 2,
                League = "Premier League",
                Home = "Liv",
                Away = "MC",
                OldValue = new DateTime(2021, 06, 23, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                NewValue = new DateTime(2022, 09, 03, 15, 00, 00).ToString("MM/dd/yyyy hh:mm tt"),
                ReceivedUserIds = new List<int> { 2, 3 },
                MessageTypeId = (int)MsgTemplateCode.UpdateKOTime
            }
        };
    }
}