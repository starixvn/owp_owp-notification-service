using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.NewLeague.Criteria;

public class GetStartTimeCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchAssignment_GetStartTimeForNightShift";

    public override bool IsValid() => true;
}