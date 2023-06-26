using Fanex.Data.Repository;
using OWP.TicketNotification.Criteria;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification;

public interface TicketRepository
{
    Task<IEnumerable<MessageUnreadCounter>> GetMessageUnreadByMultipleUserId(IList<int> userIds);

    Task InsertNewTicketLeagueNotification(InsertNewTicketLeagueNotificationCriteria criteria);

    Task InsertNewTicketLiveIncidentNotification(InsertNewTicketLiveIncidentNotificationCriteria criteria);

    Task InsertNewTicketMatchNotification(InsertNewTicketMatchNotificationCriteria criteria);

    Task InsertNewTicketOtherNotification(InsertNewTicketOtherNotificationCriteria criteria);

    Task InsertNewTicketSupportNotification(InsertNewTicketSupportNotificationCriteria criteria);

    Task InsertNewTicketLogWorkNotification(InsertNewTicketLogWorkNotificationCriteria criteria);

    Task InsertNewRecipientNotification(InsertNewRecipientNotificationCriteria criteria);

    Task InsertNewMessageNotification(InsertNewMessageNotificationCriteria criteria);
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

    public async Task InsertNewTicketLeagueNotification(InsertNewTicketLeagueNotificationCriteria criteria)
    => await dynamicRepository.ExecuteAsync(criteria);

    public async Task InsertNewTicketLiveIncidentNotification(InsertNewTicketLiveIncidentNotificationCriteria criteria)
        => await dynamicRepository.ExecuteAsync(criteria);

    public async Task InsertNewTicketMatchNotification(InsertNewTicketMatchNotificationCriteria criteria)
        => await dynamicRepository.ExecuteAsync(criteria);

    public async Task InsertNewTicketOtherNotification(InsertNewTicketOtherNotificationCriteria criteria)
        => await dynamicRepository.ExecuteAsync(criteria);

    public async Task InsertNewTicketSupportNotification(InsertNewTicketSupportNotificationCriteria criteria)
        => await dynamicRepository.ExecuteAsync(criteria);

    public async Task InsertNewTicketLogWorkNotification(InsertNewTicketLogWorkNotificationCriteria criteria)
       => await dynamicRepository.ExecuteAsync(criteria);

    public async Task InsertNewRecipientNotification(InsertNewRecipientNotificationCriteria criteria)
        => await dynamicRepository.ExecuteAsync(criteria);

    public async Task InsertNewMessageNotification(InsertNewMessageNotificationCriteria criteria)
        => await dynamicRepository.ExecuteAsync(criteria);
}
