using Microsoft.Extensions.DependencyInjection;

namespace OWP.RabbitMQ;

public static class DependenceInjection
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<MessageServer>();

        return services;
    }
}
