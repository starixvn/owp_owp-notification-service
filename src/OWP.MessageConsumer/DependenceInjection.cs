using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OWP.MessageConsumer.Models;

namespace OWP.MessageConsumer;

public static class DependenceInjection
{
    public static IServiceCollection AddMessageConsumer(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<RabbitMQSetting>(c => config.GetSection(RabbitMQSetting.RabbitMQ));
        services.AddSingleton<Consumer>();
        return services;
    }
}
