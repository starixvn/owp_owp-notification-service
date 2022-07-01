namespace OWP.PreMatchNotification.Assignment.Models
{
    public class MyAssignmentDetectorModel
    {
        public string LeagueName { get; set; }

        public string OldAssignment { get; set; }

        public string NewAssignment { get; set; }

        public string BettypeName { get; set; }

        public int? UserIdRemoved { get; set; }

        public int? UserIdNewAssign { get; set; }

        public bool? IsDay { get; set; }
    }
}