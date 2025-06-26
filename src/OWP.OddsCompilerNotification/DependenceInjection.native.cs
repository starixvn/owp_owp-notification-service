using Microsoft.Extensions.DependencyInjection;
using OWP.OddsCompilerNotification.Services;

namespace OWP.OddsCompilerNotification;

public static class DependenceInjection
{
    public static IServiceCollection AddOddsCompiler(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(OddsCompilerNotificationHandler).Assembly));

        services.AddTransient<OddsCompilerMessageNotificationService, OddsCompilerMessageNotificationServiceImp>();
        services.AddTransient<OddsCompilerRepository, OddsCompilerRepositoryImp>();

        return services;
    }
}

