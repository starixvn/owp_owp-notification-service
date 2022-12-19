namespace OWP.AssignmentUploadNotification.Models
{
    public class ChangedMatchSnapshot
    {
        public GeneralMatch OldMatch { get; set; }

        public GeneralMatch NewMatch { get; set; }

        public AssignmentSnapshot OldSnapshot { get; set; }

        public AssignmentSnapshot NewSnapshot { get; set; }
    }
}
