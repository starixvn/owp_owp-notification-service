using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OWP.TicketNotification.Handlers;
using OWP.TicketNotification.Services;

namespace OWP.TicketNotification;

public static class DependenceInjection
{
    public static IServiceCollection AddTicket(this IServiceCollection services)
    {
        services.AddMediatR(typeof(NewMessageHandler).Assembly);

        services.AddTransient<TicketRepository, TicketRepositoryImp>();
        services.AddTransient<MessageNotificationService, MessageNotificationServiceImp>();
        services.AddTransient<NotificationService, NotificationServiceImp>();
        services.AddTransient<NewRecipientMessageDBWriter, NewRecipientMessageDBWriterImp>();
        services.AddTransient<NewMessageDBWriter, NewMessageDBWriterImp>();
        services.AddTransient<NewTicketLiveIncidentMessageDbWriter, NewTicketLiveIncidentMessageDbWriterImp>();
        services.AddTransient<NewTicketLeagueMessageDBWriter, NewTicketLeagueMessageDBWriterImp>();
        services.AddTransient<NewTicketMatchMessageDBWriter, NewTicketMatchMessageDBWriterImp>();
        services.AddTransient<NewTicketOtherMessageDBWriter, NewTicketOtherMessageDBWriterImp>();
        services.AddTransient<NewTicketSupportMessageDBWriter, NewTicketSupportMessageDBWriterImp>();
        services.AddTransient<NewTicketLogWorkMessageDBWriter, NewTicketLogWorkMessageDBWriterImp>();

        return services;
    }
}
