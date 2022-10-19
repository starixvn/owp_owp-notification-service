namespace OWP.PreMatchNotification.Shared.Models;

public class Assignee : IEqualityComparer<Assignee>
{
    public int AssigneeId { get; set; }

    public string AssigneeName { get; set; }

    public int PreMatchLeagueId { get; set; }

    public int OWPLeagueId { get; set; }

    public bool Equals(Assignee x, Assignee y)
    {
        return x.AssigneeId == y.AssigneeId && x.AssigneeName == y.AssigneeName;
    }

    public int GetHashCode(Assignee obj)
    {
        return AssigneeId;
    }
}