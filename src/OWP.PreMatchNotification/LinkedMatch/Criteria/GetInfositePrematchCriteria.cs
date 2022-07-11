using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.LinkedMatch.Criteria;

public class GetInfositePrematchCriteria : CriteriaBase
{
    public DateTime EventFromDateTime { get; set; }

    public DateTime EventToDateTime { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchNotification_QueryInfoSiteMatchesByDate";

    public override bool IsValid() => true;
}