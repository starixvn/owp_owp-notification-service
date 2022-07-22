using MediatR;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests.TicketCenter;
using OWP.TicketNotification.Constants;
using OWP.TicketNotification.Models;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification.Handlers
{
    internal class NewTicketOtherHandler : IRequestHandler<NewTicketOtherNotificationRequest, string>
    {
        private readonly NewTicketOtherMessageDBWriter writer;
        private readonly MessageNotificationService messageNotificationService;

        public NewTicketOtherHandler(NewTicketOtherMessageDBWriter writer, MessageNotificationService messageNotificationService)
        {
            this.writer = writer;
            this.messageNotificationService = messageNotificationService;
        }

        public async Task<string> Handle(NewTicketOtherNotificationRequest request, CancellationToken cancellationToken)
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
                EventType = (int)EventType.Other,
            };

            var messages = await messageNotificationService.CreateMessageNotification(notificationSource);

            var result = messages.Select(m => new
            {
                NotificationId = Guid.NewGuid().ToString(),
                Message = m
            }).ToList();

            return JsonConvert.SerializeObject(result);
        }
    }
}
