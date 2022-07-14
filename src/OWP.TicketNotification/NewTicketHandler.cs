using Fanex.NAP.Client.Helpers.Web;
using MediatR;
using OWP.Core.NotificationRequests;
using OWP.Core.Services;
using OWP.MessageConsumer.Constants;
using OWP.MessagePublisher;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification;

public class NewTicketHandler : IRequestHandler<NewTicketNotificationRequest, Unit>
{
    private readonly TicketRepository ticketRepository;
    private readonly QueryMessageService messageService;
    private readonly Publisher publisher;


    public NewTicketHandler(TicketRepository ticketRepository, INapSession napSession, QueryMessageService messageService, Publisher publisher)
    {
        this.ticketRepository = ticketRepository;
        this.messageService = messageService;
        this.publisher = publisher;
    }

    public async Task<Unit> Handle(NewTicketNotificationRequest request, CancellationToken cancellationToken)
    {
        var userTuple = await messageService.QueryBySession(request.SessionId);
        var userIds = userTuple.Item1.Select(x => x.UserId).ToList();
        var awaiter = ticketRepository.GetMessageUnreadByMultipleUserId(userIds).ConfigureAwait(true);
        var messageUnread = awaiter.GetAwaiter().GetResult();
        var messagesDic = userTuple.Item2.Select(x => new NotificationModel
        {
            UserId = x.UserId,
            IsImportant = x.IsImportant,
            MessageType = x.MessageType,
            MessageContent = x.MessageContent,
            TicketId = request.TicketId,
            EventType = request.EventType,
            TotalMessageUnread = messageUnread?.FirstOrDefault(m => m.UserId == x.UserId)?.Total,
            IsNewMessage = request.IsNewMessage
        }).DistinctBy(x => new
        {
            x.UserId,
            x.IsImportant,
            x.MessageType,
            x.MessageContent,
            x.TicketId,
            x.EventType,
            x.TotalMessageUnread,
            x.IsNewMessage
        }).ToDictionary(x => x.UserId.ToString(), x => x);

        List<dynamic> messages = new List<dynamic>();

        foreach (var userId in messagesDic.Keys)
        {
            var message = messagesDic[userId];
            var result = new
            {
                notificationId = Guid.NewGuid(),
                message
            };

            messages.Add(result);

        }

        publisher.PushMessage(messages, routingKey: RoutingKey.TicketCenter);

        return Unit.Value;
    }
}
