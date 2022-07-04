using Fanex.Data.Repository;

namespace OWP.OtherSportNotification.Criteria;

public class GetAllMatchesByDateTimeListCriteria : CriteriaBase
{
    public string EventDates { get; set; }

    public override string GetSettingKey() => "OWP_OSAssignmentUpload_GetAllMatchesByDateList";

    public override bool IsValid() => true;
}
