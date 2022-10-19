using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OWP.LiveDetailNotification.Services;

namespace OWP.LiveDetailNotification;

public static class DependenceInjection
{
    public static IServiceCollection AddLiveDetail(this IServiceCollection services)
    {
        services.AddMediatR(typeof(LiveDetailNotificationHandler).Assembly);
        services.AddTransient<LiveDetailMessageNotificationService, LiveDetailMessageNotificationServiceImp>();
        services.AddTransient<NotificationService, NotificationServiceImp>();

        return services;
    }
}

