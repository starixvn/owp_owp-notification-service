namespace OWP.PreMatchNotification.NewLeague.Models;
public class CacheInfo
{
    public string Actor { get; set; }

    public IList<string> Leagues { get; set; }= new List<string>();

}
