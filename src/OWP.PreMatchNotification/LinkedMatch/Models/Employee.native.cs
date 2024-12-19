namespace OWP.PreMatchNotification.LinkedMatch.Models;

public class Employee
{
    public Dictionary<int, List<InfositePrematchLeague>> PrematchInfosites { get; set; } = new();

    public List<int> AllPrematchEmployees { get; set; } = [];

    public List<int> AllLSTEmployees { get; set; } = [];
}