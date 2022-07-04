using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OWP.MessageConsumer;

public static class DependenceInjection
{
    public static IServiceCollection AddMessageConsumer(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<Consumer>();
        return services;
    }
}
