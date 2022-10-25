using Fanex.Data.Repository;
using OWP.Core.Criteria;
using OWP.Core.Models;

namespace OWP.Core.Services;


public interface QueryMessageService
{
    Task<Tuple<IList<User>, IList<MessageModel>>> QueryBySession(string sessionId);
}

public class QueryMessageServiceImp : QueryMessageService
{
    private readonly IDynamicRepository repository;

    public QueryMessageServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Tuple<IList<User>, IList<MessageModel>>> QueryBySession(string sessionId)
    {
        var criteria = new GetMessageBySessionCriteria
        {
            SessionId = sessionId
        };

        var result = await repository.FetchMultipleAsync<User, MessageModel>(criteria);
        return new Tuple<IList<User>, IList<MessageModel>>(result.Item1.ToList(), result.Item2.ToList());
    }
}
