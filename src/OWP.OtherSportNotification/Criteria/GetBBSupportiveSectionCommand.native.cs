using Fanex.Data.Repository;

namespace OWP.OtherSportNotification.Criteria;

public class GetBBSupportiveSectionCommand : CriteriaBase
{
    public override string GetSettingKey() => "OWP_OSAssignmentUpload_GetSectionBBSupportive";

    public override bool IsValid() => true;
}