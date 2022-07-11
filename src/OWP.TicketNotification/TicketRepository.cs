using Fanex.Data.Repository;
using OWP.TicketNotification.Critera;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification;

public interface TicketRepository
{
    Task<IEnumerable<MessageUnreadCounter>> GetMessageUnreadByMultipleUserId(IList<int> userIds);
}

public class TicketRepositoryImp : TicketRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public TicketRepositoryImp(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<IEnumerable<MessageUnreadCounter>> GetMessageUnreadByMultipleUserId(IList<int> userIds)
    {
        var messageUnread = new List<MessageUnreadCounter>();

        foreach (var userId in userIds)
        {
            var result = await dynamicRepository.FetchAsync<int>(new CountUnreadMessageCriteria
            {
                UserId = userId
            });
            var totalUnreadMessage = result.FirstOrDefault();
            messageUnread.Add(new MessageUnreadCounter
            {
                UserId = userId,
                Total = totalUnreadMessage
            });
        }
        return messageUnread;
    }
}
