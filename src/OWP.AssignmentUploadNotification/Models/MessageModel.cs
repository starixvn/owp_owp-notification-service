namespace OWP.AssignmentUploadNotification.Models;

public class MessageModel
{
    public string MessageContent { get; set; }

    public bool IsImportant { get; set; }

    public bool IsWatcher { get; set; }

    public int MessageType { get; set; }

    public List<string> ReceivedUserIds { get; set; }

    public string ReceivedUserIdList { get; set; }

    public string ReceivedUserNameList { get; set; }
}