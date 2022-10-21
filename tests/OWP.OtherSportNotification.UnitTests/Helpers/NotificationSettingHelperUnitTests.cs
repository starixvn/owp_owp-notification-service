using OWP.OtherSportNotification.Helpers;

namespace OWP.OtherSportNotification.UnitTests.Helpers
{
    public class NotificationSettingHelperUnitTests
    {
        [Theory]
        [InlineData(0, true, 2)]
        [InlineData(103, false, 1)]
        [InlineData(117, false, 5)]
        [InlineData(113, false, 11)]
        public void GetNotificationSettingIdForAssignee(int messageTypeId, bool isIndirectChangedAssignment, int expected)
        {
            var actual = NotificationSettingHelper.GetNotificationSettingIdForAssignee(messageTypeId, isIndirectChangedAssignment);

            Assert.Equal(expected, actual);
        }
    }
}