using OWP.Core.Hubs;
using OWP.MessageConsumer;
using OWP.MessageConsumer.Models;
using OWP.NotificationService.Extensions;
using OWP.OtherSportNotification;
using OWP.TicketNotification;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFanexServices(builder.Configuration);

builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));

builder.Services.AddSignalR();

builder.Services.AddCoreServices();
builder.Services.AddMessageConsumer(builder.Configuration);
builder.Services.AddTicket();
builder.Services.AddOtherSport();





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
