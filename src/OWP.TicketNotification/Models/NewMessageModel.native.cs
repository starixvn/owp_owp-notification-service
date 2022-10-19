namespace OWP.TicketNotification.Models;

public class NewMessageModel
{
    public string Message { get; set; }

    public int? FileId { get; set; }

    public int TicketEventType { get; set; }

    public string TicketTitle { get; set; }
    public int TicketId { get; set; }
    public int ActorId { get; set; }
}
