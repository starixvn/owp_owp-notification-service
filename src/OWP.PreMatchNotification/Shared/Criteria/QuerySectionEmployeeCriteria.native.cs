using Fanex.Data.Repository;

namespace OWP.PreMatchNotification.Criteria;

public class QuerySectionEmployeeCriteria : CriteriaBase
{
    public string Section { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchNotification_QueryEmployeeBySection";

    public override bool IsValid() => true;
}