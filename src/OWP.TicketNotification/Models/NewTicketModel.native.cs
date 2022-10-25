namespace OWP.TicketNotification.Models;

public class NewTicketModel
{
    public int ActorId { get; set; }

    public int TicketId { get; set; }

    public string Comment { get; set; }

    public string TicketType { get; set; }

    public string Username { get; set; }
}

public class NewTicketLiveIncidentModel : NewTicketModel
{
    public string OwpLeagueName { get; set; }

    public string OwpHomeName { get; set; }

    public string OwpAwayName { get; set; }
}