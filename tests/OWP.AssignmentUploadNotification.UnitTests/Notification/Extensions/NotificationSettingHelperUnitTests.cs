namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Extensions;

using AssignmentUploadNotification.Extensions;
using Constants;
using Xunit;

public class NotificationSettingHelperUnitTests
{
    [Theory]
    [InlineData((int)MsgTemplateCode.AssignmentDelete, 0, false, (int)NotificationSettingCode.MyAssignmentUpdatedId)]
    [InlineData((int)MsgTemplateCode.AssignmentInsert, 0, false, (int)NotificationSettingCode.MyAssignmentUpdatedId)]
    [InlineData((int)MsgTemplateCode.OtherAssignmentDelete, 0, false, (int)NotificationSettingCode.MyAssignmentUpdatedId)]
    [InlineData((int)MsgTemplateCode.OtherAssignmentInsert, (int)MajorBetType.LA, true, (int)NotificationSettingCode.AssigneeLAUpdatedId)]
    [InlineData((int)MsgTemplateCode.AssignmentInsert, (int)MajorBetType.XMins, true, (int)NotificationSettingCode.AssigneeTraderUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditMaxbet, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditSpread, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditTV, 0, false, (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditChannel, 0, false, (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWB, 0, false, (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditOtherProvider, 0, false, (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditProvider, 0, false, (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditAgent, 0, false, (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditPhone, 0, false, (int)NotificationSettingCode.AssigneeProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite1, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite2, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite3, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite4, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditNoOfHDP, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditRemark, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditRound, 0, false, (int)NotificationSettingCode.AssigneeMatchInfoUpdatedId)]
    public void GetNotificationSettingIdForAssignee_Always_ReturnNotificationSettingId(int messageTypeId, int bettypeId, bool isIndirectChangedAssignment, int expected)
    {
        // Act
        var actual = NotificationSettingHelper.GetNotificationSettingIdForAssignee(messageTypeId, bettypeId, isIndirectChangedAssignment);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData((int)MsgTemplateCode.AssignmentDelete, 0, (int)NotificationSettingCode.WatcherTraderUpdatedId)]
    [InlineData((int)MsgTemplateCode.AssignmentInsert, 0, (int)NotificationSettingCode.WatcherTraderUpdatedId)]
    [InlineData((int)MsgTemplateCode.OtherAssignmentDelete, (int)MajorBetType.LA, (int)NotificationSettingCode.WatcherLAUpdatedId)]
    [InlineData((int)MsgTemplateCode.OtherAssignmentInsert, 0, (int)NotificationSettingCode.WatcherTraderUpdatedId)]
    [InlineData((int)MsgTemplateCode.OtherAssignmentInsert, (int)MajorBetType.LA, (int)NotificationSettingCode.WatcherLAUpdatedId)]
    [InlineData((int)MsgTemplateCode.AssignmentInsert, (int)MajorBetType.XMins, (int)NotificationSettingCode.WatcherTraderUpdatedId)]
    [InlineData((int)MsgTemplateCode.AssignmentDelete, (int)MajorBetType.AllInOne, (int)NotificationSettingCode.WatcherTraderUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditMaxbet, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditSpread, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditTV, 0, (int)NotificationSettingCode.WatcherProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditChannel, 0, (int)NotificationSettingCode.WatcherProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWB, 0, (int)NotificationSettingCode.WatcherProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditOtherProvider, 0, (int)NotificationSettingCode.WatcherProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditProvider, 0, (int)NotificationSettingCode.WatcherProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditAgent, 0, (int)NotificationSettingCode.WatcherProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditPhone, 0, (int)NotificationSettingCode.WatcherProviderInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite1, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite2, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite3, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditWebsite4, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditNoOfHDP, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditRemark, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    [InlineData((int)MsgTemplateCode.EditRound, 0, (int)NotificationSettingCode.WatcherMatchInfoUpdatedId)]
    public void GetNotificationSettingIdForWatcher_Always_ReturnNotificationSettingId(int messageTypeId, int bettypeId, int expected)
    {
        // Act
        var actual = NotificationSettingHelper.GetNotificationSettingIdForWatcher(messageTypeId, bettypeId);

        // Assert
        Assert.Equal(expected, actual);
    }
}