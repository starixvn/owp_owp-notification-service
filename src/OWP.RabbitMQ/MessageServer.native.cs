using Fanex.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OWP.RabbitMQ.Handler;
using OWP.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.RabbitMQ;

public class MessageServer(
    IOptions<RabbitMQSetting> options,
    ILogger logger,
    NotificationHandler notificationHandler)
    : IHostedService
{
    private readonly RabbitMQSetting setting = options.Value;

    public async Task<bool> ConsumeRequestQueue()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                UserName = setting.UserName,
                Password = setting.Password,
                HostName = setting.Hosting,
                Port = setting.Port,
                VirtualHost = setting.VirtualHost
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

           await channel.QueueDeclareAsync(queue: setting.RequestQueue, durable: true, exclusive: false, autoDelete: false);

            await channel.BasicQosAsync(0, 4, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            await channel.BasicConsumeAsync(queue: setting.RequestQueue, autoAck: true, consumer: consumer);

            consumer.ReceivedAsync += OnReceived;
        }
        catch (Exception ex)
        {
           await logger.ErrorAsync(ex.Message, ex);
           return false;
        }

        return true;
    }

    private async Task OnReceived(object model, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var props = eventArgs.BasicProperties;

            var body = eventArgs.Body.ToArray();
            var requestMessage = Encoding.UTF8.GetString(body);

            await notificationHandler.Handle(props.Type, requestMessage);

        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex.Message, ex);
        }

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConsumeRequestQueue();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}