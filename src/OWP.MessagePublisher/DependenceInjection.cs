using Microsoft.Extensions.DependencyInjection;

namespace OWP.MessagePublisher
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddMessagePublisher(this IServiceCollection services)
        {
            services.AddSingleton<Publisher>();
            return services;
        }
    }
}
