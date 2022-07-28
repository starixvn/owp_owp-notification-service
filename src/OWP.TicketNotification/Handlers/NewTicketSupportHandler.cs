using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers
{
    internal class NewTicketSupportHandler : IRequestHandler<NewTicketSupportNoticationRequest, string>
    {
        private readonly NewTicketSupportMessageDBWriter writer;
        private readonly MessageNotificationService messageNotificationService;

        public NewTicketSupportHandler(NewTicketSupportMessageDBWriter writer, MessageNotificationService messageNotificationService)
        {
            this.writer = writer;
            this.messageNotificationService = messageNotificationService;
        }

        public async Task<string> Handle(NewTicketSupportNoticationRequest request, CancellationToken cancellationToken)
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

            return JsonConvert.SerializeObject(messages);
        }
    }

}