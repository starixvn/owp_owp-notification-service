using Microsoft.AspNetCore.Diagnostics;
using OWP.RabbitMQ;

namespace OWP.NotificationService.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
    {
        var rabbitService = app.ApplicationServices.GetService<MessageServer>();

        var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        lifetime!.ApplicationStarted.Register(() => rabbitService!.ConsumeRequestQueue());

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

