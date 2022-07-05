using OWP.Core.Hubs;
using OWP.MessageConsumer;
using OWP.MessageConsumer.Models;
using OWP.NotificationService.Extensions;
using OWP.OtherSportNotification;
using OWP.TicketNotification;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCorsPolicy();
builder.Services.AddSignalR();

builder.Services.AddFanexServices(builder.Configuration);

// Add config
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));

// Add internal service
builder.Services.AddCoreServices();
builder.Services.AddMessageConsumer(builder.Configuration);
builder.Services.AddTicket();
builder.Services.AddOtherSport();


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
