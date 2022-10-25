namespace OWP.OtherSportNotification.Models;

public class AssignmentSnapshot
{
    public Dictionary<int, MatchInfo> Matches { get; set; }

    public Dictionary<int, List<MatchSetting>> MatchSettings { get; set; }

    public Dictionary<int, List<MatchAssignment>> MatchAssignments { get; set; }

    public AssignmentSnapshot()
    {
        Matches = new Dictionary<int, MatchInfo>();
        MatchSettings = new Dictionary<int, List<MatchSetting>>();
        MatchAssignments = new Dictionary<int, List<MatchAssignment>>();
    }
}

public class MatchInfo
{
    public double MatchNo { get; set; }

    public int OSMatchId { get; set; }

    public string OfferLive { get; set; }

    public string MatchStatus { get; set; }

    public int? AdminsiteMatchId { get; set; }

    public string Reporter { get; set; }

    public string Sport { get; set; }

    public DateTime DateTime { get; set; }

    public string Home { get; set; }

    public bool IsNeutral { get; set; }

    public string Away { get; set; }

    public string League { get; set; }

    public string Category { get; set; }

    public string NoHDP { get; set; }

    public string MatchOffer { get; set; }

    public string AutoLink { get; set; }

    public string Provider { get; set; }

    public string OtherProvider { get; set; }

    public string Service { get; set; }

    public string IPTV { get; set; }

    public string WB { get; set; }

    public string Remark { get; set; }

    public string Username { get; set; }
}
