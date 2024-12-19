using OWP.TicketNotification.Models;
using System.Collections.Immutable;

namespace OWP.TicketNotification.Constants;

public static class MessageTemplate
{
    #region MessageTemplate

    private static readonly ImmutableList<NotificationTemplate> notificationTemplates = ImmutableList<NotificationTemplate>.Empty.AddRange(
    [
        new NotificationTemplate
            {
                NotificationType = (int)NotificationTemplateCode.NewTicket,
                NotificationContent = "New ticket <b>#<ticketId></b> <Event>: <comment> by <username>",
                IsImportant = true
            },

            new NotificationTemplate
            {
                NotificationType = (int)NotificationTemplateCode.AddRecipient,
                NotificationContent = "You have been added to ticket <b>#<ticketId></b> <Event> by <username>",
                IsImportant = true
            },
            new NotificationTemplate
            {
                NotificationType = (int)NotificationTemplateCode.NewMessage,
                NotificationContent = "New message from ticket <b>#<ticketId></b> <Event>: \"<comment>\" by <username>",
                IsImportant = true
            },
            new NotificationTemplate
            {
                NotificationType = (int)NotificationTemplateCode.NewAttachment,
                NotificationContent = "New attachment from ticket <b>#<ticketId></b> <Event> by <username>",
                IsImportant = true
            }
    ]);

    #endregion MessageTemplate

    public static readonly ImmutableDictionary<int, NotificationTemplate> NotificationTemplateMap =
        notificationTemplates.ToDictionary(
        m => m.NotificationType,
        m => m).ToImmutableDictionary();
}
