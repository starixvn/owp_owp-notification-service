namespace OWP.AssignmentUploadNotification.Models;

public class AssignmentSnapshot
{
    public Dictionary<int, GeneralMatch> Matches { get; set; } = new();

    public Dictionary<int, List<MatchSetting>> MatchSettings { get; set; } = new();

    public Dictionary<int, List<MatchAssignment>> Assignments { get; set; } = new();
}