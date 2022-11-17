namespace OWP.Core.NotificationRequests.AssignmentUpload
{
    public class AssignmentUploadNotifyRequest : NotificationRequest
    {
        public DateTime EventDate { get; set; }

        public int ActorId { get; set; }

        public bool IsUploadFMAndAIO { get; set; }
    }
}