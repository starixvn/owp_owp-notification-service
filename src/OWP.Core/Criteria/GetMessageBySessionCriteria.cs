using Fanex.Data.Repository;

namespace OWP.Core.Criteria;

public class GetMessageBySessionCriteria : CriteriaBase
{
    public string SessionId { get; set; }

    public override string GetSettingKey() => "OWP_MessageGetBySessionId";

    public override bool IsValid() => !string.IsNullOrEmpty(SessionId);
}