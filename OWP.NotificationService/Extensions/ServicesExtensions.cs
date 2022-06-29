
using Fanex.Data;
using Fanex.Data.SqlServer;

namespace OWP.NotificationService.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddFanexServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connections = configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .AsEnumerable()
                .ToDictionary(
                    x => x.GetValue<string>("Name"),
                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
                );

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                .WithSqlServer()
                .UseDefaultDbSettingProvider(resourcePath: "~/App_Data")
                .Run();

            return services;
        }
    }
}
