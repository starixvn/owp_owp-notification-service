using OWP.Core.Extensions;

namespace OWP.OtherSportNotification.Constants;

public static class MessageType
{
    private static readonly Dictionary<string, int> MessageTypeMap = new()
    {
            { "MatchNo", (int)MsgTemplateCode.EditMatchNo },
            { "MatchStatus", (int)MsgTemplateCode.EditMatchStatus },
            { "Category", (int)MsgTemplateCode.EditCategory },
            { "MatchOffer", (int)MsgTemplateCode.EditMatchOffer },
            {"NoHDP",  (int)MsgTemplateCode.EditNoOfHDP},
            { "AutoLink", (int)MsgTemplateCode.EditAutoLink },
            { "OtherProvider", (int)MsgTemplateCode.EditOtherProvider },
            { "IPTV", (int)MsgTemplateCode.EditIPTV },
            { "WB", (int)MsgTemplateCode.EditWB },
            { "Remark", (int)MsgTemplateCode.EditRemark },
            { "MaxBet", (int)MsgTemplateCode.EditMaxbet },
            { "Reporter", (int)MsgTemplateCode.EditReporter },
            { "Provider", (int)MsgTemplateCode.EditProvider },
            { "IsNeutral", (int)MsgTemplateCode.EditNeutral }
        };

    public static int GetMessageTypeIdForChangedMatchInfo(string property)
        => MessageTypeMap.GetValue(property, 0);
}

