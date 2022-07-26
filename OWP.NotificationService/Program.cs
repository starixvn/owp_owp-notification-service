using OWP.NotificationService.Extensions;
using OWP.OtherSportNotification;
using OWP.PreMatchNotification;
using OWP.RabbitMQ;
using OWP.RabbitMQ.Models;
using OWP.TicketNotification;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddFanexServices(builder.Configuration);
builder.Services.AddRedisServices(builder.Configuration);

builder.Services.AddNapServices(builder.Configuration);

// Add config
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));

// Add internal service
builder.Services.AddCoreServices();
builder.Services.AddRabbitMQ();
builder.Services.AddTicket();
builder.Services.AddOtherSport();
builder.Services.AddPrematchNotificationServices();

builder.Services.AddHostedService<MessageServer>();

var app = builder.Build();

await app.RunAsync("http://0.0.0.0:6000");
