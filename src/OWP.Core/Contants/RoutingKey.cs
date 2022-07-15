namespace OWP.Core.Contants;

public static class RoutingKey
{
    public const string LinkedLgaMatch = "linked-lga-match";
    public const string PrematchNewLeague = "prematch-new-league";
    public const string InfositeMatchInfo = "infosite-match-info";
    public const string PrematchAssignment = "prematch-assignment";
    public const string TicketCenter = "ticket-center";
    public const string TicketCenterNewMessage = "ticket-center-new-message";
    public const string TicketCenterNewRecipient = "ticket-center-new-recipient";
    public const string TicketCenterNewTicketEvent = "ticket-center-new-ticket-event";
    public const string TicketCenterNewTicketLeague = "ticket-center-new-ticket-league";
    public const string TicketCenterNewTicketOther = "ticket-center-new-ticket-other";
    public const string TicketCenterNewTicketSupport = "ticket-center-new-ticket-support";
    public const string OtherSport = "other-sport";

    public static readonly string[] Keys = new string[]
    {
        LinkedLgaMatch,
        PrematchNewLeague,
        InfositeMatchInfo,
        PrematchAssignment,
        TicketCenter,
        OtherSport,
        TicketCenterNewMessage,
        TicketCenterNewRecipient,
        TicketCenterNewTicketEvent,
        TicketCenterNewTicketLeague,
        TicketCenterNewTicketOther,
        TicketCenterNewTicketSupport
    };
}
