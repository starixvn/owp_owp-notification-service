namespace OWP.AssignmentUploadNotification.Constants;

using System.Collections.Generic;
using OWP.Core.Extensions;

public static class MessageType
{
    private static readonly Dictionary<string, int> MessageTypeMap = new()
    {
        { "OtherTime", (int)MsgTemplateCode.EditOtherTime },
        { "Home", (int)MsgTemplateCode.EditHomeName },
        { "KickOffTime", (int)MsgTemplateCode.EditKOTime },
        { "Agent", (int)MsgTemplateCode.EditAgent },
        { "Channel", (int)MsgTemplateCode.EditChannel },
        { "Division", (int)MsgTemplateCode.EditDivision },
        { "MatchNo", (int)MsgTemplateCode.EditMatchNo },
        { "MatchOffer", (int)MsgTemplateCode.EditMatchOffer },
        { "MatchStatus", (int)MsgTemplateCode.MatchStatus },
        { "NoOfHDP", (int)MsgTemplateCode.EditNoOfHDP },
        { "Phone", (int)MsgTemplateCode.EditPhone },
        { "Provider", (int)MsgTemplateCode.EditProvider },
        { "OtherProvider", (int)MsgTemplateCode.EditOtherProvider },
        { "Remark", (int)MsgTemplateCode.EditRemark },
        { "Round", (int)MsgTemplateCode.EditRound },
        { "TV", (int)MsgTemplateCode.EditTV },
        { "WB", (int)MsgTemplateCode.EditWB },
        { "Website1", (int)MsgTemplateCode.EditWebsite1 },
        { "Website2", (int)MsgTemplateCode.EditWebsite2 },
        { "Website3", (int)MsgTemplateCode.EditWebsite3 },
        { "Website4", (int)MsgTemplateCode.EditWebsite4 },
        { "Assignment", (int)MsgTemplateCode.AssignmentDelete },
        { "AssignmentInsert", (int)MsgTemplateCode.AssignmentInsert },
        { "Maxbet", (int)MsgTemplateCode.EditMaxbet },
        { "Spread", (int)MsgTemplateCode.EditSpread },
        { "MinorMatchStatus", (int)MsgTemplateCode.MinorMatchStatus },
        { "OtherAssignmentDelete", (int)MsgTemplateCode.OtherAssignmentDelete },
        { "OtherAssignmentInsert", (int)MsgTemplateCode.OtherAssignmentInsert },
        { "OtherAssignmentUpdate", (int)MsgTemplateCode.OtherAssignmentUpdate },
        { "ChangeOfAssignment", (int)MsgTemplateCode.ChangeOfAssignment }
    };

    public static int GetMessageTypeIdForChangedMatchInfo(string property)
    {
        return MessageTypeMap.GetValue(property, 0);
    }
}