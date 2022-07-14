using Fanex.NAP.Client;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using OWP.Core.Hubs;
using OWP.Core.Models;
using OWP.MessageConsumer;
using OWP.MessagePublisher;
using OWP.NotificationService.Extensions;
using OWP.OtherSportNotification;
using OWP.PreMatchNotification;
using OWP.TicketNotification;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCorsPolicy();
builder.Services.AddSignalR();

builder.Services.AddFanexServices(builder.Configuration);
builder.Services.AddRedisServices(builder.Configuration);

builder.Services.AddControllerAuthorization();
builder.Services.AddCookieAuthentication(builder.Configuration);

builder.Services.AddSessionConfig(builder.Configuration);
builder.Services.AddNapServices(builder.Configuration);

// Add config
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.RabbitMQ));

// Add internal service
builder.Services.AddCoreServices();
builder.Services.AddMessageConsumer(builder.Configuration);
builder.Services.AddMessagePublisher();
builder.Services.AddTicket();
builder.Services.AddOtherSport();
builder.Services.AddPrematchNotificationServices();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCookiePolicy();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseException();

app.UseRabbitListener();

app.UseCors();

app.UseEndpoints(c =>
{
    c.MapControllers();
    c.MapHub<ChangedInfoNotifyHubImp>("/changedInfoNotifyHub");
});

NapClientManager
  .UseConfig(app.Services.GetService<INapSettings>())
  .UseSession(app.Services.GetService<INapSession>());


app.Run();
