namespace OWP.AssignmentUploadNotification.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.Xml;

public interface CommandRepository
{
    Task InsertMessage(List<MessageModel> messages, string sessionId, int userId);
    Task WriteLog(OwpLogCriteria owpLogCriteria);
}

public class CommandRepositoryImp(IDynamicRepository repository) : CommandRepository
{
    public async Task InsertMessage(List<MessageModel> messages, string sessionId, int userId)
    {
        var criteria = new InsertMessageCommand
        {
            Messages = XmlConverter.Serialize(messages),
            CreatedByUserId = userId,
            SessionId = sessionId
        };

        await repository.ExecuteAsync(criteria);
    }

    public async Task WriteLog(OwpLogCriteria owpLogCriteria)
    {
        await repository.ExecuteAsync(owpLogCriteria);
    }
}