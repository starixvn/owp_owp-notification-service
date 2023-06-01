namespace OWP.AssignmentUploadNotification.Criteria;

using Fanex.Data.Repository;

public class GetNotificationSettingCommand : CriteriaBase
{
    public override string GetSettingKey() => "OWP_AssignmentUpload_GetNotificationSetting";

    public override bool IsValid() => true;
}