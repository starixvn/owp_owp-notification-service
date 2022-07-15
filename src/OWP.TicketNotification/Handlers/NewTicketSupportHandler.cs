using MediatR;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.MessageConsumer.Constants;
using OWP.MessagePublisher;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers
{
    internal class NewTicketSupportHandler : IRequestHandler<NewTicketSupportNoticationRequest, Unit>
    {
        private readonly NewTicketSupportMessageDBWriter writer;
        private readonly Publisher publisher;
        private readonly MessageNotificationService messageNotificationService;

        public NewTicketSupportHandler(NewTicketSupportMessageDBWriter writer, Publisher publisher, MessageNotificationService messageNotificationService)
        {
            this.writer = writer;
            this.publisher = publisher;
            this.messageNotificationService = messageNotificationService;
        }


        public async Task<Unit> Handle(NewTicketSupportNoticationRequest request, CancellationToken cancellationToken)
        {
            var notificationMessage = new NewTicketModel
            {
                TicketId = request.TicketId,
                Comment = request.Comment,
                ActorId = request.ActorId
            };

            var messageId = await writer.Execute(notificationMessage, NotificationTemplateCode.NewTicket);

            var notificationSource = new NotificationSource
            {
                SessionId = messageId,
                TicketId = request.TicketId,
                EventType = (int)EventType.Support,
            };
            var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

            // publish
            publisher.PushMessage(messages, routingKey: RoutingKey.TicketCenterNewTicketSupport);

            return Unit.Value;
        }
    }

}