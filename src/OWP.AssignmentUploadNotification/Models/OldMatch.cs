namespace OWP.AssignmentUploadNotification.Models
{
    public class OldMatch : IEqualityComparer<OldMatch>
    {
        public int OWPMatchId { get; set; }

        public int InfoSiteMatchId { get; set; }

        public bool IsSaba { get; set; }

        public int? MinorBettypeId { get; set; }

        public bool Equals(OldMatch x, OldMatch y)
            => x.InfoSiteMatchId == y.InfoSiteMatchId && x.MinorBettypeId == y.MinorBettypeId;

        public int GetHashCode(OldMatch obj)
            => MinorBettypeId.HasValue ? InfoSiteMatchId * MinorBettypeId.Value : InfoSiteMatchId;
    }
}