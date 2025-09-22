using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.NewLeague.Criteria;

public class QueryShiftPreMatchCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public DateTime EventFromDateTime { get; set; }

    public DateTime EventToDateTime { get; set; }

    public bool IsDay { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchAssignment_QueryDayShift";

    public override bool IsValid() => true;
}