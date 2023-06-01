namespace OWP.AssignmentUploadNotification.Models;

public class MessageTemplate
{
    public int MessageType { get; set; }

    public string MessageContent { get; set; }

    public bool IsImportant { get; set; }
}