namespace OWP.OtherSportNotification.UnitTests.Services;
public class ChangedMatchSettingGeneratorUnitTests
{
    [Fact]
    public void GetListValueChangedOfMatchSetting_OldvalueHasMaxbetChange_NewvalueHasMaxbetChange()
    {
        var oldMatchSetting = new MatchSetting
        {
            MaxBet = 120.00m,
            Spread = 0.32m
        };
        var newMatchSetting = new MatchSetting
        {
            MaxBet = 11000.00m,
            Spread = 0.31m
        };
        var expected = new List<ChangedValue>
            {
              new()
              {
                  BetTypeName = "FT (HDP)",
                  OldValue = "120",
                  NewValue = "11000",
                  MessageTypeId = (int)MsgTemplateCode.EditMaxbet,
                  OSMatchId = 123
                },
              new()
              {
                  BetTypeName = "FT (HDP)",
                  OldValue = "32%",
                  NewValue = "31%",
                  MessageTypeId = (int)MsgTemplateCode.EditSpread,
                  OSMatchId = 123
                }
            };

        var actual = ChangedMatchSettingGeneratorImp.GetListValueChangedOfMatchSetting(oldMatchSetting, newMatchSetting, "FT (HDP)", 123);

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void GetListMatchChangeMaxBet_WithBettypeFTHDP_ReturnListMatch_Has_MaxbetChanged()
    {
        var oldMatchSettings = new List<MatchSetting>
            {
                new()
                {
                    OSMatchId=123,
                    MaxBet=11000.00m,
                    Spread= 0.02m,
                    BetTypeName= "FT (HDP)"
                }
            };
        var newMatchSettings = new List<MatchSetting>
            {
                new()
                {
                    OSMatchId=123,
                    MaxBet=12000.00m,
                    Spread=0.021m,
                    BetTypeName= "FT (HDP)"
                }
            };
        var expected = new List<ChangedValue>
            {
                new()
                {
                    MessageTypeId=(int)MsgTemplateCode.EditMaxbet,
                    BetTypeName="FT (HDP)",
                    OldValue="11000",
                    NewValue="12000",
                    OSMatchId=123
                },
                new()
                {
                    MessageTypeId=(int)MsgTemplateCode.EditSpread,
                    BetTypeName="FT (HDP)",
                    OldValue="2%",
                    NewValue="2.1%",
                    OSMatchId=123
                }
            };

        var actual = ChangedMatchSettingGeneratorImp.GetListValueChangedWithDifferentBettype(oldMatchSettings, newMatchSettings, 123);

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void CreateListMatchSettingChange_Old_New_Snapshot_Has_Diff_MatchSetting_Return_ListMatchChanged()
    {
        var changedMatchSettingGeneratorImp = new ChangedMatchSettingGeneratorImp();
        var snapshot = MockChangeSnapshot();
        var assignees = new List<Assignee>
            {
                new()
                {
                    AssigneeId=8,
                    AssigneeName="sa"
                }
            };
        var expected = GetExpect();

        var actual = changedMatchSettingGeneratorImp.Create(snapshot, 123, assignees);

        DeepAssert.Equal(expected, actual);
    }

    private ChangedMatchSnapshot MockChangeSnapshot()
    => new()
    {
        OldSnapshot = new AssignmentSnapshot
        {
            MatchSettings = new Dictionary<int, List<MatchSetting>>
                {
                        {123, [
                                new MatchSetting
                                {
                                    OSMatchId = 123,
                                    BetTypeName = "FT (HDP)",
                                    MaxBet = 12000.00m,
                                    Spread = 0.21m
                                },

                                new MatchSetting
                                {
                                    OSMatchId = 123,
                                    BetTypeName = "FT (OE)",
                                    MaxBet = 17000.00m,
                                    Spread = 0.01m
                                }
                            ]
                        }
                }
        },
        NewSnapshot = new AssignmentSnapshot
        {
            MatchSettings = new Dictionary<int, List<MatchSetting>>
                {
                        {123, [
                                new MatchSetting
                                {
                                    OSMatchId = 123,
                                    BetTypeName = "FT (HDP)",
                                    MaxBet = 11000.00m,
                                    Spread = 0.22m
                                },

                                new MatchSetting
                                {
                                    OSMatchId = 123,
                                    BetTypeName = "FT (OE)",
                                    MaxBet = 19000.00m,
                                    Spread = 0.02m
                                }
                            ]
                        }
                },
            Matches = new Dictionary<int, MatchInfo>
                {
                        {123, new MatchInfo
                        {
                            MatchNo=1234,
                            League="League",
                            Home="United",
                            Away= "Ha Noi"
                        } }
                }
        }
    };

    private List<ChangedValue> GetExpect()
    =>
    [
        new()
        {
            UserId = 8,
            Username = "sa",
            BetTypeName = "FT (HDP)",
            EventName = "United -vs- Ha Noi",
            League = "League",
            MatchNo = "1234",
            MessageTypeId = (int)MsgTemplateCode.EditMaxbet,
            NewValue = "11000",
            OldValue = "12000",
            OSMatchId = 123
        },

        new()
        {
            UserId = 8,
            Username = "sa",
            BetTypeName = "FT (HDP)",
            EventName = "United -vs- Ha Noi",
            League = "League",
            MatchNo = "1234",
            MessageTypeId = (int)MsgTemplateCode.EditSpread,
            NewValue = "22%",
            OldValue = "21%",
            OSMatchId = 123
        },

        new()
        {
            UserId = 8,
            Username = "sa",
            BetTypeName = "FT (OE)",
            EventName = "United -vs- Ha Noi",
            League = "League",
            MatchNo = "1234",
            MessageTypeId = (int)MsgTemplateCode.EditMaxbet,
            NewValue = "19000",
            OldValue = "17000",
            OSMatchId = 123
        },

        new()
        {
            UserId = 8,
            Username = "sa",
            BetTypeName = "FT (OE)",
            EventName = "United -vs- Ha Noi",
            League = "League",
            MatchNo = "1234",
            MessageTypeId = (int)MsgTemplateCode.EditSpread,
            NewValue = "2%",
            OldValue = "1%",
            OSMatchId = 123
        }
    ];
}