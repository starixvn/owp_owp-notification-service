using OWP.MessageConsumer;
using IHostApplicationLifetime = Microsoft.Extensions.Hosting.IHostApplicationLifetime;

namespace OWP.NotificationService.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
    {
        var rabbitService = app.ApplicationServices.GetService<Consumer>();
        var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        lifetime!.ApplicationStarted.Register(() => rabbitService!.ReceiveMessageFromQueue());

        return app;
    }
}
