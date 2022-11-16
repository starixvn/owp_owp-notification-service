namespace OWP.AssignmentUploadNotification.Models
{
    using System.Collections.Generic;

    public class Assignee : IEqualityComparer<Assignee>
    {
        public int AssigneeId { get; set; }

        public string AssigneeName { get; set; }

        public bool Equals(Assignee x, Assignee y)
        {
            return x.AssigneeId == y.AssigneeId && x.AssigneeName == y.AssigneeName;
        }

        public int GetHashCode(Assignee obj)
        {
            return AssigneeId;
        }
    }
}
