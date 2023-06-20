namespace OWP.PreMatchNotification.Assignment.Models;

public class AssignmentSnapshot
{
    public string LeagueName { get; set; }

    public int PreMatchLeagueId { get; set; }

    public bool? IsDay { get; set; }

    public string Hdp { get; set; }

    public int? HdpUserId { get; set; }

    public string Ou { get; set; }

    public int? OuUserId { get; set; }

    public string HdpTakeOver { get; set; }

    public int? HdpTakeOverUserId { get; set; }

    public string OuTakeOver { get; set; }

    public int? OuTakeOverUserId { get; set; }

    public string OtherHdp { get; set; }

    public int? OtherHdpUserId { get; set; }

    public string OtherOu { get; set; }

    public int? OtherOuUserId { get; set; }

    public string OtherHdpTakeOver { get; set; }

    public int? OtherHdpTakeOverUserId { get; set; }

    public string OtherOuTakeOver { get; set; }

    public int? OtherOuTakeOverUserId { get; set; }
}