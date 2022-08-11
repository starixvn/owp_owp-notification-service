using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.InfoSiteMatchInfo.Criteria;

public class GetInfoSiteMatchesByDateRangeCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchNotification_QueryInfoSiteMatchesByDateRange";

    public override bool IsValid() => true;
}