using OWP.MessageConsumer;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace OWP.NotificationService.Extensions
{
    public static class AppExtensions
    {
        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            var rabbitService = app.ApplicationServices.GetService<Consumer>();
            var lifeTimeApp = app.ApplicationServices.GetService<IApplicationLifetime>();

            lifeTimeApp!.ApplicationStarted.Register(() => rabbitService!.ReceiveMessageFromQueue());

            return app;
        }
    }
}
