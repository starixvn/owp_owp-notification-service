using Microsoft.Extensions.DependencyInjection;
using OWP.RabbitMQ.Consumer;
using OWP.RabbitMQ.Publisher;

namespace OWP.RabbitMQ;

public static class DependenceInjection
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<MessagePublisher>();
        services.AddSingleton<MessageConsumer>();
        services.AddSingleton<MessageServer>();

        return services;
    }
}
