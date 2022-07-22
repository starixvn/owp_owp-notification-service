using Microsoft.Extensions.DependencyInjection;
using OWP.RabbitMQ.Handler;

namespace OWP.RabbitMQ;

public static class DependenceInjection
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<MessageServer>();
        services.AddTransient<NotificationHander, SendNotificationHanderImp>();

        return services;
    }
}
