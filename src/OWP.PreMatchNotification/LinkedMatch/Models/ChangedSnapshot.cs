namespace OWP.PreMatchNotification.LinkedMatch.Models;

public class ChangedSnapshot
{
    public List<GeneralMatch> DeletedMatches { get; set; }

    public List<GeneralMatch> InsertedMatches { get; set; }

    public List<GeneralMatch> ChangedMatches { get; set; }

    public List<GeneralMatch> OldSnapshot { get; set; }

    public List<GeneralMatch> NewSnapshot { get; set; }
}