using Fanex.Logging.Extensions;
using ILogger = Fanex.Logging.ILogger;

namespace OWP.NotificationService.CrossCuttingConcern;

public class ErrorHandler(RequestDelegate next, ILogger logger, IConfiguration configuration)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        var logMessage = new LogMessage(exception.Message)
            .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
            .ToString();
        await logger.ErrorAsync(logMessage);
    }
}