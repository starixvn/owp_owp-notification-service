using MediatR;
using OWP.Core.Hubs;
using OWP.MessageConsumer;
using OWP.MessageConsumer.Handler;
using OWP.NotificationHandler.Hubs;
using OWP.NotificationHandler.Services;
using OWP.NotificationService.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(NotificationRequest).GetTypeInfo().Assembly);
builder.Services.AddSignalR();
builder.Services.Configure<RabbitMQSetting>(
    builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));
builder.Services.AddSingleton<Consumer>();
builder.Services.AddTransient<NotifyEngine, NotifyEngineImp>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRabbitListener();

app.MapControllers();

app.MapHub<ChangedInfoNotifyHubImp>("/changedInfoNotifyHub");
app.MapHub<NotificationHub>("/changedInfoNotifyHub");

app.Run();
