namespace OWP.PreMatchNotification.NewLeague.Models
{
    public class PreMatchLeague : IEqualityComparer<PreMatchLeague>
    {
        public int PreMatchLeagueId { get; set; }

        public int OWPLeagueId { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsDay { get; set; }

        public string OWPLeagueName { get; set; }

        public bool Equals(PreMatchLeague x, PreMatchLeague y)
            => x.OWPLeagueId == y.OWPLeagueId;

        public int GetHashCode(PreMatchLeague obj)
            => PreMatchLeagueId * 13 + OWPLeagueId * 17;
    }
}