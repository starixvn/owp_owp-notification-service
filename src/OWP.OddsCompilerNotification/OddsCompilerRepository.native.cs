using Fanex.Data.Repository;
using Olympus.OWP.Log.Data.Repository.Criteria;

namespace OWP.OddsCompilerNotification;

public interface OddsCompilerRepository
{

    Task WriteLog(OwpLogCriteria owpLogCriteria);
}

public class OddsCompilerRepositoryImp(IDynamicRepository repository) : OddsCompilerRepository
{

    public async Task WriteLog(OwpLogCriteria owpLogCriteria)
    {
        await repository.ExecuteAsync(owpLogCriteria);
    }
}
