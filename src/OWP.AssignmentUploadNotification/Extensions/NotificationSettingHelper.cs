namespace OWP.AssignmentUploadNotification.Extensions;

using System.Collections.Generic;
using Constants;

public static class NotificationSettingHelper
{
    public static int GetNotificationSettingIdForAssignee(int messageTypeId, int bettypeId, bool isIndirectChangedAssignment)
    {
        if (isIndirectChangedAssignment)
        {
            return GetIdForChangeAssignmentOfAssignee(bettypeId);
        }

        if (IsUpdateAssignment(messageTypeId))
        {
            return (int)NotificationSettingCode.MyAssignmentUpdatedId;
        }

        if (IsMatchSetting(messageTypeId))
        {
            return (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId;
        }

        if (IsProviderInfoUpdate(messageTypeId))
        {
            return (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId;
        }

        return (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId;
    }

    private static int GetIdForChangeAssignmentOfAssignee(int bettypeId)
    {
        return bettypeId == (int)MajorBetType.LA
            ? (int)NotificationSettingCode.AssigneeLAUpdatedId
            : (int)NotificationSettingCode.AssigneeTraderUpdatedId;
    }

    public static bool IsUpdateAssignment(int messageTypeId)
    {
        var updateAssignmentMessageTypeIds = new List<int>
        {
            (int)MsgTemplateCode.AssignmentDelete,
            (int)MsgTemplateCode.OtherAssignmentDelete,
            (int)MsgTemplateCode.AssignmentInsert,
            (int)MsgTemplateCode.OtherAssignmentInsert
        };

        return updateAssignmentMessageTypeIds.Contains(messageTypeId);
    }

    public static bool IsProviderInfoUpdate(int messageTypeId)
    {
        var providerMessageTypeIds = new List<int> {
            (int)MsgTemplateCode.EditAgent,
            (int)MsgTemplateCode.EditChannel,
            (int)MsgTemplateCode.EditOtherProvider,
            (int)MsgTemplateCode.EditProvider,
            (int)MsgTemplateCode.EditTV,
            (int)MsgTemplateCode.EditWB,
            (int)MsgTemplateCode.EditPhone
        };

        return providerMessageTypeIds.Contains(messageTypeId);
    }

    public static bool IsMatchSetting(int messageTypeId)
    {
        var matchSettingMessageTypeIds = new List<int>
        {
            (int)MsgTemplateCode.EditMaxbet,
            (int)MsgTemplateCode.EditSpread
        };

        return matchSettingMessageTypeIds.Contains(messageTypeId);
    }
}