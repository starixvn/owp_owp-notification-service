namespace OWP.OtherSportNotification.Models;

public class InsertMessageModel
{
    public bool IsImportant { get; set; }

    public int MessageType { get; set; }

    public string MessageContent { get; set; }

    public List<string> ReceivedUserIds { get; set; }

    public string ReceivedUserIdList { get; set; }

    public bool IsWatcher { get; set; }
}
