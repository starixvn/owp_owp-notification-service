using Fanex.Data.Repository;

namespace OWP.OtherSportNotification.Criteria;

public class GetSessionReceivedWatcherCommand : CriteriaBase
{
    public override string GetSettingKey() => "OWP_AssignmentUpload_GetSectionMemberOfUserReceivedWatcher";

    public override bool IsValid() => true;
}