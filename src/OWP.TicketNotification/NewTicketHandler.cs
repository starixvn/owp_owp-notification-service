using Fanex.NAP.Client.Helpers.Web;
using MediatR;
using OWP.Core.NotificationRequests;
using OWP.Core.Services;
using OWP.TicketNotification.Models;

namespace OWP.TicketNotification;

public class NewTicketHandler : IRequestHandler<NewTicketNotificationRequest, Unit>
{
    private readonly NotificationService notificationService;
    private readonly TicketRepository ticketRepository;
    private readonly INapSession napSession;


    public NewTicketHandler(NotificationService notificationService, TicketRepository ticketRepository, INapSession napSession)
    {
        this.notificationService = notificationService;
        this.ticketRepository = ticketRepository;
        this.napSession = napSession;
    }

    public async Task<Unit> Handle(NewTicketNotificationRequest request, CancellationToken cancellationToken)
    {
        var currentSession = napSession;
        await notificationService.Notify(request.SessionId, messageTuple =>
        {
            var userIds = messageTuple.Item1
              .Select(x => x.UserId)
              .ToList();

            var awaiter = ticketRepository
                      .GetMessageUnreadByMultipleUserId(userIds)
                      .ConfigureAwait(true);

            var messageUnread = awaiter.GetAwaiter().GetResult();

            return messageTuple.Item2.Select(x => new NotificationModel
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
        });

        return Unit.Value;
    }
}
