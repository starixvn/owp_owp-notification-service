namespace OWP.AssignmentUploadNotification.Models;

public class AssignmentSnapshot
{
    public Dictionary<int, GeneralMatch> Matches { get; set; }

    public Dictionary<int, List<MatchSetting>> MatchSettings { get; set; }

    public Dictionary<int, List<MatchAssignment>> Assignments { get; set; }

    public AssignmentSnapshot()
    {
        Matches = new Dictionary<int, GeneralMatch>();
        MatchSettings = new Dictionary<int, List<MatchSetting>>();
        Assignments = new Dictionary<int, List<MatchAssignment>>();
    }
}