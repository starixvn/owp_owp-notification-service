namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria;
    using Fanex.Data.Repository;
    using Models;
    using Olympus.Xml;

    public interface CommandRepository
    {
        Task InsertMessage(List<MessageModel> messages, string sessionId, int userId);
    }

    public class CommandRepositoryImp : CommandRepository
    {
        private readonly IDynamicRepository repository;

        public CommandRepositoryImp(
            IDynamicRepository repository)
        {
            this.repository = repository;
        }

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
    }
}
