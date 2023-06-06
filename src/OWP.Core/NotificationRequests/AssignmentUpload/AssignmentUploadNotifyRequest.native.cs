namespace OWP.Core.NotificationRequests.AssignmentUpload;

public class AssignmentUploadNotifyRequest : NotificationRequest
{
    public DateTime EventDate { get; set; }

    public bool IsUploadFMAndAIO { get; set; }

    public string UserName { get; set; }

    public int ActorId { get; set; }

}