using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.Assignment.Criteria
{
    public class QueryAssignmentByPrematchLeagueIdCriteria : CriteriaBase
    {
        public int PreMatchLeagueId { get; set; }

        public override string GetSettingKey() => "OWP_PreMatchNotification_QueryAssignmentByPreMatchLeagueId";

        public override bool IsValid() => true;
    }
}