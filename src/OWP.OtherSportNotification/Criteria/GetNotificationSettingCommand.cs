using Fanex.Data.Repository;

namespace OWP.OtherSportNotification.Criteria;

public class GetNotificationSettingCommand : CriteriaBase
{
    public override string GetSettingKey() => "OWP_AssignmentUpload_GetNotificationSetting";

    public override bool IsValid() => true;
}
