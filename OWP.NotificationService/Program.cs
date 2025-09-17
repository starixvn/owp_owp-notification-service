using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using OWP.AssignmentUploadNotification;
using OWP.LiveDetailNotification;
using OWP.NotificationService;
using OWP.NotificationService.CrossCuttingConcern;
using OWP.NotificationService.Extensions;
using OWP.OddsCompilerNotification;
using OWP.OtherSportNotification;
using OWP.PreMatchNotification;
using OWP.PreMatchNotification.NewLeague.Services;
using OWP.RabbitMQ;
using OWP.RabbitMQ.Models;
using OWP.TicketNotification;
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args);
var services= builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddSignalRedis(builder.Configuration);
services.AddFanexSql(builder.Configuration);
services.AddFanexLogSentry(configuration);
services.AddRedisServices(builder.Configuration);

// AutoMapper
services.AddAutoMapper();

// Add config
services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));

// Add internal service
services.AddHttpClient();
services.AddCoreServices();
services.AddRabbitMQ();
services.AddTicket();
services.AddOtherSport();
services.AddLiveDetail();
services.AddPrematchNotificationServices();
services.AddAssignmentUpload();
services.AddOddsCompiler();
services.AddSingleton<GarbageCollectorJob>();

services.AddHostedService<MessageServer>();
services.AddHostedService<GarbageCollectorJob>();



var app = builder.Build();
app.UseMiddleware<ErrorHandler>();
var logger = app.Services.GetRequiredService<Fanex.Logging.ILogger>();
await logger.InfoAsync("Environment: "+ configuration["Env"]);
app.Run(async context =>
{
    var request = context.Request.Path.HasValue
    ? context.Request.Path.Value
    : string.Empty;
#if DEBUG
    var shiftNotificationService = context.RequestServices.GetService<NightShiftNotificationService>();
    await shiftNotificationService.SendNotificationForNewLeague();
    await context.Response.WriteAsync($"OWP Notification Services. Request Path = {request}");
#endif
});

await app.RunAsync();



