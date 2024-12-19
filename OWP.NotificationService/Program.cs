using Fanex.Data.SqlServer;
using OWP.AssignmentUploadNotification;
using OWP.LiveDetailNotification;
using OWP.NotificationService;
using OWP.NotificationService.Extensions;
using OWP.OtherSportNotification;
using OWP.PreMatchNotification;
using OWP.RabbitMQ;
using OWP.RabbitMQ.Models;
using OWP.TicketNotification;
var builder = WebApplication.CreateBuilder(args);
var services= builder.Services;
// Add services to the container.

services.AddControllers();
services.AddSignalRedis(builder.Configuration);
services.AddFanexSql(builder.Configuration);
services.AddRedisServices(builder.Configuration);

services.AddNapServices(builder.Configuration);

// AutoMapper
services.AddAutoMapper();

// Add config
services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));

// Add internal service
services.AddCoreServices();
services.AddRabbitMQ();
services.AddTicket();
services.AddOtherSport();
services.AddLiveDetail();
services.AddPrematchNotificationServices();
services.AddAssignmentUpload();
services.AddSingleton<GarbageCollectorJob>();

services.AddHostedService<MessageServer>();
services.AddHostedService<GarbageCollectorJob>();

var app = builder.Build();

app.Run(async context =>
{
    var request = context.Request.Path.HasValue
    ? context.Request.Path.Value
    : string.Empty;
    await context.Response.WriteAsync($"OWP Worker Services. Request Path = {request}");
});

await app.RunAsync();



