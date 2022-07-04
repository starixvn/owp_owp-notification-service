namespace OWP.MessageConsumer.Constants;

public static class RoutingKey
{
    public const string LgaMatch = "linked-lga-match";
    public const string PrematchNewLeague = "prematch-new-league";
    public const string InfositeMatchInfo = "infosite-match-info";
    public const string TicketCenter = "ticket-center";
    public const string OtherSport = "other-sport";

    public static readonly string[] Keys = new string[] { LgaMatch, PrematchNewLeague, InfositeMatchInfo, TicketCenter, OtherSport };
}
