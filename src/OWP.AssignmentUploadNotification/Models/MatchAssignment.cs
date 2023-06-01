namespace OWP.AssignmentUploadNotification.Models;

public class MatchAssignment
{
    public int MatchId { get; set; }

    public int BetType { get; set; }

    public int AssignmentType { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public string AssignmentNote { get; set; }

    public MatchAssignment()
    {
        AssignmentNote = string.Empty;
    }
}