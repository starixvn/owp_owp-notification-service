using Fanex.Data.Repository;

namespace TicketNotification.Critera;

public class CountUnreadMessageCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public override string GetSettingKey() => "OWP_CountUnreadMessageByUserId";

    public override bool IsValid() => true;
}
