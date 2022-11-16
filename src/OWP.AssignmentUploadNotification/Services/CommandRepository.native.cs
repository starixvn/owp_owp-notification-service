namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria;
    using Fanex.Data.Repository;
    using Fanex.NAP.Client.Helpers.Web;
    using Models;
    using Olympus.Xml;

    public interface CommandRepository
    {
        Task InsertMessage(List<MessageInfo> messages, string sessionId);
    }

    public class CommandRepositoryImp : CommandRepository
    {
        private readonly IDynamicRepository repository;
        private readonly INapSession currentSession;

        public CommandRepositoryImp(
            INapSession currentSession,
            IDynamicRepository repository)
        {
            this.currentSession = currentSession;
            this.repository = repository;
        }

        public async Task InsertMessage(List<MessageInfo> messages, string sessionId)
        {
            var criteria = new InsertMessageCommand
            {
                Messages = XmlConverter.Serialize(messages),
                CreatedByUserId = currentSession.UserId,
                SessionId = sessionId
            };

            await repository.ExecuteAsync(criteria);
        }
    }
}
