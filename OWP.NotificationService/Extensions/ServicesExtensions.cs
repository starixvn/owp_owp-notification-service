
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.NAP.Client.AspNetCore.Configuration;
using Fanex.NAP.Client.AspNetCore.Session;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
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

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            return services;
        }


        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<OWP.Core.Services.NotificationService, NotifyEngineImp>();
            services.AddTransient<QueryMessageService, QueryMessageServiceImp>();
            services.AddTransient<IDynamicRepository, DynamicRepository>();

            return services;
        }

        public static IServiceCollection AddNapServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<Fanex.Logging.ILogger, Logger>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<INapSettings>(new NapSettings(configuration));
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<IGroupService, GroupService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ISecurityService, SecurityService>();

            return services;
        }

        public static IServiceCollection AddSessionConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:SessionTimeOut").Value));
                options.Cookie.Name = "Fanex.OWP.Session";
            });

            return services;
        }

        public static IServiceCollection AddControllerAuthorization(this IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            return services;
        }

        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
                            options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
                            options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:CookieTimeOut").Value));
                            options.Cookie = new CookieBuilder
                            {
                                Name = "Fanex.OWP.Cookie",
                                SecurePolicy = CookieSecurePolicy.SameAsRequest
                            };
                            options.Events.OnSigningOut = context =>
                            {
                                context.Response.Redirect(options.LoginPath);
                                return Task.CompletedTask;
                            };
                        });

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