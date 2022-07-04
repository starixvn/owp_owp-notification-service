using OWP.OtherSportNotification.Constants;

namespace OWP.OtherSportNotification.Helpers
{
    public static class NotificationSettingHelper
    {
        public static int GetNotificationSettingIdForAssignee(int messageTypeId, bool isIndirectChangedAssignment)
        {
            if (isIndirectChangedAssignment)
            {
                return (int)NotificationSettingCode.AssigneeTraderUpdatedId;
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
                (int)MsgTemplateCode.EditOtherProvider,
                (int)MsgTemplateCode.EditProvider,
                (int)MsgTemplateCode.EditWB
            };

            return providerMessageTypeIds.Contains(messageTypeId);
        }

        public static bool IsMatchSetting(int messageTypeId)
        {
            var matchSettingMessageTypeIds = new List<int>
            {
                (int)MsgTemplateCode.EditMaxbet
            };

            return matchSettingMessageTypeIds.Contains(messageTypeId);
        }
    }
}
