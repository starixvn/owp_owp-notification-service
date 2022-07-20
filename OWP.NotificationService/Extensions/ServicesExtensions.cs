
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.NAP.Client.AspNetCore.Session;
using OWP.Core.Services;

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

        public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSetting = configuration.GetSection(RedisSetting.RedisConnectKey).Get<RedisSetting>();
            services.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
            {
                ConnectionString = redisSetting.ConnectionString,
                ApplicationName = redisSetting.AppName
            }));

            return services;
        }

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<QueryMessageService, QueryMessageServiceImp>();
            services.AddTransient<IDynamicRepository, DynamicRepository>();

            return services;
        }

        public static IServiceCollection AddNapServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<Fanex.Logging.ILogger, Logger>();

            return services;
        }
    }
}


public class RedisSetting
{
    public static string RedisConnectKey = "RedisConnect";
    public string ConnectionString { get; set; }
    public string AppName { get; set; }
}