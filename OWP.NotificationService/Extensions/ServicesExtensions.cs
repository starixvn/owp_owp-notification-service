using AutoMapper;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using OWP.Core.Models;
using OWP.Core.Services;
using StackExchange.Redis;
using Notify = OWP.Core.Services;

namespace OWP.NotificationService.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AssignmentUploadNotification.AutoMapperProfile());
        });

        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

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
        services.AddTransient<Notify.NotificationService, NotificationServiceImp>();
        return services;
    }

    public static IServiceCollection AddSignalRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSetting = configuration.GetSection(RedisSetting.RedisConnectKey).Get<RedisSetting>();
        services.AddSignalR().AddStackExchangeRedis(redisSetting.ConnectionString, options =>
        {
            options.Configuration.ChannelPrefix = RedisChannel.Literal(redisSetting.ChannelPrefix);
        });
        return services;
    }

    public static IServiceCollection AddHttpClientFactory(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("OWPChatAdmin", client =>
        {
            var chatUrl = configuration["ChatServer:Url"];
            client.BaseAddress = new Uri(chatUrl);
            var adminToken = configuration["ChatServer:AdminToken"];
            var adminId = configuration["ChatServer:AdminUserId"];
            client.BaseAddress = new Uri(chatUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-Auth-Token", adminToken);
            client.DefaultRequestHeaders.Add("X-User-Id", adminId);
        });

        services.AddHttpClient("OWPChatBot", client =>
        {
            var chatUrl = configuration["ChatServer:Url"];
            client.BaseAddress = new Uri(chatUrl);
           
            var botToken = configuration["ChatServer:BotToken"];
            var botId = configuration["ChatServer:BotUserId"];
            client.BaseAddress = new Uri(chatUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-Auth-Token", botToken);
            client.DefaultRequestHeaders.Add("X-User-Id", botId);
        });
        return services;
    }
}