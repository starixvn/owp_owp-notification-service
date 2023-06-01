using Fanex.Data.Repository;

namespace OWP.OtherSportNotification.Criteria;

public class InsertMessageCommand : NonQueryCommand
{
    public string Messages { get; set; }

    public int CreatedByUserId { get; set; }

    public string SessionId { get; set; }

    public override string GetSettingKey() => "OWP_AssignmentUpload_MessageInsert";

    public override bool IsValid() => true;
}