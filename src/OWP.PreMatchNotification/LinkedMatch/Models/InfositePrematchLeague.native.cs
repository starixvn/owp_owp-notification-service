namespace OWP.PreMatchNotification.LinkedMatch.Models;

public class InfositePrematchLeague
{
    public int InfoSiteMatchId { get; set; }

    public int PreMatchLeagueId { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public DateTime EventDateTime { get; set; }

    public List<int> PrematchAssigneeIds { get; set; } = new();
}