using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.Shared.Criteria;

public class QueryLeagueAssigneeCriteria : CriteriaBase
{
    public string PreMatchLeagueIds { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchNotification_QueryLeagueAssignee";

    public override bool IsValid() => true;
}