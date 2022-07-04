using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OWP.TicketNotification;

public static class DependenceInjection
{
    public static IServiceCollection AddTicket(this IServiceCollection services)
    {
        services.AddTransient<TicketRepository, TicketRepositoryImp>();
        services.AddMediatR(typeof(NewTicketHandler).GetTypeInfo().Assembly);
        return services;
    }
}
