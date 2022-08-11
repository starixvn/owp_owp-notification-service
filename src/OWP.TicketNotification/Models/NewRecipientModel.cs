namespace OWP.TicketNotification.Models;

public class NewRecipientModel
{
    public string TicketTitle { get; set; }
    public int TicketId { get; set; }
    public int ActorId { get; set; }

    public int RecipientId { get; set; }

    public int RecipientType { get; set; }
}
