using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.Criteria;

public class QueryLeagueAssigneeCriteriaByOWPLeagueIds : CriteriaBase
{
    public string OWPLeagueIds { get; set; }

    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchNotification_QueryLeagueAssigneeByOWPLeagueIds";

    public override bool IsValid() => true;
}