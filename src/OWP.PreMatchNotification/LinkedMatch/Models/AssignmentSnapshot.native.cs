namespace OWP.PreMatchNotification.LinkedMatch.Models;

public class AssignmentSnapshot
{
    public Dictionary<int, GeneralMatch> Matches { get; set; } = new();
}

public class GeneralMatch
{
    public int MatchId { get; set; }
    public int InfoSiteMatchId { get; set; }
    public int ParentMatchId { get; set; }
    public int MatchType { get; set; }
    public string MinorBettypeName { get; set; }
    public string LeagueName { get; set; }
    public string Home { get; set; }
    public string Away { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime EventDateTime { get; set; }
    public TimeSpan OtherTime { get; set; }
    public TimeSpan KickOffTime { get; set; }
    public string Agent { get; set; }
    public string Channel { get; set; }
    public string Division { get; set; }
    public string MatchNo { get; set; }
    public string MatchOffer { get; set; }
    public string MatchStatus { get; set; }
    public string NoOfHDP { get; set; }
    public string Note { get; set; }
    public string Phone { get; set; }
    public string Provider { get; set; }
    public string OtherProvider { get; set; }
    public string Remark { get; set; }
    public string Round { get; set; }
    public string TV { get; set; }
    public string WB { get; set; }
    public string Website1 { get; set; }
    public string Website2 { get; set; }
    public string Website3 { get; set; }
    public string Website4 { get; set; }
}