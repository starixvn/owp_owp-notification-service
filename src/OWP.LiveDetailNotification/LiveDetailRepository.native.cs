using System;
using Fanex.Data.Repository;
using Olympus.OWP.Log.Data.Repository.Criteria;

namespace OWP.LiveDetailNotification;

public interface LiveDetailRepository
{

    Task WriteLog(OwpLogCriteria owpLogCriteria);
}

public class LiveDetailRepositoryImp(IDynamicRepository repository) : LiveDetailRepository
{

    public async Task WriteLog(OwpLogCriteria owpLogCriteria)
    {
        await repository.ExecuteAsync(owpLogCriteria);
    }
}
