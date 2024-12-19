using OWP.RabbitMQ;

namespace OWP.NotificationService.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
    {
        var rabbitService = app.ApplicationServices.GetService<MessageServer>();

        var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() => rabbitService.ConsumeRequestQueue().ConfigureAwait(false));

        return app;
    }
}

