using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.InfoSiteMatchInfo.Criteria;

public class GetLinkedOWPMatchCriteria : CriteriaBase
{
    public string InfositeMatchIds { get; set; }

    public override string GetSettingKey() => "OWP_PrematchNotification_GetLinkedOWPMatch";

    public override bool IsValid() => true;
}