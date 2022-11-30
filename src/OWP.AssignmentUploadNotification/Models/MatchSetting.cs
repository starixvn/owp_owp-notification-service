namespace OWP.AssignmentUploadNotification.Models
{
    public class MatchSetting
    {
        public int MatchId { get; set; }

        public int BetType { get; set; }

        public decimal? Spread { get; set; }

        public decimal? MaxBet { get; set; }
    }
}