using Fanex.Data.Repository;

namespace OWP.AssignmentUploadNotification.Criteria
{
    public class GetAllMatchesByDateSelCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public override string GetSettingKey() => "OWP_AssignmentUpload_AllMatchIdsByDateSel";

        public override bool IsValid() => true;
    }
}