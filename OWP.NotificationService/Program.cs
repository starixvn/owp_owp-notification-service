using Fanex.Data.Repository;
using MediatR;
using OWP.Core.Hubs;
using OWP.Core.NotificationRequests;
using OWP.Core.Services;
using OWP.MessageConsumer;
using OWP.NotificationService.Extensions;
using OWP.TicketNotification;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFanexServices(builder.Configuration);

builder.Services.AddMediatR(typeof(NotificationRequest).GetTypeInfo().Assembly);
builder.Services.AddSignalR();
builder.Services.Configure<RabbitMQSetting>(
builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));
builder.Services.AddSingleton<Consumer>();
builder.Services.AddTransient<NotificationService, NotifyEngineImp>();
builder.Services.AddTransient<QueryMessageService, QueryMessageServiceImp>();
builder.Services.AddTransient<IDynamicRepository, DynamicRepository>();
builder.Services.AddTransient<TicketRepository, TicketRepositoryImp>();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseRabbitListener();

app.UseCors();

app.UseEndpoints(c =>
{
    c.MapControllers();
    c.MapHub<ChangedInfoNotifyHubImp>("/changedInfoNotifyHub");
});


app.Run();
