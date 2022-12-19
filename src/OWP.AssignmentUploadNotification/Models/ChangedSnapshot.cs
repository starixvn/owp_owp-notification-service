namespace OWP.AssignmentUploadNotification.Models
{
    using System.Collections.Generic;

    public class ChangedSnapshot
    {
        public List<int> DeletedMatches { get; set; }

        public List<int> InsertedMatches { get; set; }

        public List<int> ChangedMatches { get; set; }

        public AssignmentSnapshot OldSnapshot { get; set; }

        public AssignmentSnapshot NewSnapshot { get; set; }
    }
}
