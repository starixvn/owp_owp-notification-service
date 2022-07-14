using Microsoft.AspNetCore.Diagnostics;
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

    public static IApplicationBuilder UseException(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                Console.WriteLine(contextFeature!.Error);
                await Task.CompletedTask;
            });
        });

        return app;
    }


}

