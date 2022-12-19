namespace OWP.AssignmentUploadNotification.Models
{
    public class SessionWatcher
    {
        public int UserId { get; set; }

        public int UserWatcherId { get; set; }

        public string UserWatcherName { get; set; }

        public int NotificationSettingId { get; set; }
    }
}
