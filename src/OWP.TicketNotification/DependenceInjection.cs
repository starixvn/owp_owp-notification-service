using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OWP.TicketNotification.Services;
using System.Reflection;

namespace OWP.TicketNotification;

public static class DependenceInjection
{
    public static IServiceCollection AddTicket(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddTransient<TicketRepository, TicketRepositoryImp>();
        services.AddTransient<MessageNotificationService, MessageNotificationServiceImp>();
        services.AddTransient<NewRecipientMessageDBWriter, NewRecipientMessageDBWriterImp>();
        services.AddTransient<MessageNotificationService, MessageNotificationServiceImp>();
        services.AddTransient<NewMessageDBWriter, NewMessageDBWriterImp>();
        services.AddTransient<NewTicketLeagueMessageDBWriter, NewTicketLeagueMessageDBWriterImp>();
        services.AddTransient<NewTicketMatchMessageDBWriter, NewTicketMatchMessageDBWriterImp>();
        services.AddTransient<NewTicketOtherMessageDBWriter, NewTicketOtherMessageDBWriterImp>();
        services.AddTransient<NewTicketSupportMessageDBWriter, NewTicketSupportMessageDBWriterImp>();

        return services;
    }
}
