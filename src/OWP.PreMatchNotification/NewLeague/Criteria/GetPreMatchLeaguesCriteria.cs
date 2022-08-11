using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.NewLeague.Criteria;

public class GetPreMatchLeaguesCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchNotification_QueryLeague";

    public override bool IsValid() => true;
}