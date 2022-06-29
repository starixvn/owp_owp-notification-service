namespace OWP.PreMatchNotification.Criteria;

using Fanex.Data.Repository;

public class QueryInfoSiteMatchByOWPLeagueIdsCriteria : CriteriaBase
{
    public string OWPLeagueIds { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchNotification_QueryInfoSiteMatchByOWPLeagueIds";

    public override bool IsValid() => true;
}