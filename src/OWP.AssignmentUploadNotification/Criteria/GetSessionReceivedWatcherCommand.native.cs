namespace OWP.AssignmentUploadNotification.Criteria;

using Fanex.Data.Repository;

public class GetSessionReceivedWatcherCommand : CriteriaBase
{
    public override string GetSettingKey() => "OWP_AssignmentUpload_GetSectionMemberOfUserReceivedWatcher";

    public override bool IsValid() => true;
}