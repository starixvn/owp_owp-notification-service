using Fanex.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OWP.RabbitMQ.Handler;
using OWP.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.RabbitMQ;

public class MessageServer : IHostedService
{
    private IModel channel;
    //private IConnection connection;

    private readonly RabbitMQSetting setting;
    private readonly ILogger logger;

    private readonly NotificationHander notificationHander;

    public MessageServer(
        IOptions<RabbitMQSetting> options,
        ILogger logger,
        NotificationHander notificationHander
        )
    {
        setting = options.Value;
        this.logger = logger;
        this.notificationHander = notificationHander;
    }

    public void ConsumeRequestQueue()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                UserName = setting.UserName,
                Password = setting.Password,
                HostName = setting.Hosting,
                Port = setting.Port,
                VirtualHost = setting.VirtualHost,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: setting.RequestQueue, durable: true, exclusive: false, autoDelete: false);
            channel.QueueDeclare(queue: setting.ReplyQueue, durable: true, exclusive: false, autoDelete: false);


            channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            channel.BasicConsume(queue: setting.RequestQueue, autoAck: true, consumer: consumer);

            consumer.Received += OnReceived;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
        }
    }

    private async Task OnReceived(object model, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var props = eventArgs.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.Type = props.Type;

            var body = eventArgs.Body.ToArray();
            var requestMessage = Encoding.UTF8.GetString(body);

            var response = await notificationHander.Handle(props.Type, requestMessage);

            var responseBytes = Encoding.UTF8.GetBytes(response);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes);

            channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);

        }

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this.ConsumeRequestQueue();
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}