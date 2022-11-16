using Fanex.Data.Repository;

namespace OWP.AssignmentUploadNotification.Criteria
{
    public class GetAllMatcheSettingsByTimeRangeCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public override string GetSettingKey() => "OWP_AssignmentUpload_AllMatchSettingsByTimeRange";

        public override bool IsValid() => true;
    }
}