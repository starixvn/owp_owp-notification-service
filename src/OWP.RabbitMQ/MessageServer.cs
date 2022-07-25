using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OWP.RabbitMQ.Handler;
using OWP.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.RabbitMQ;

public class MessageServer
{
    private IModel channel;

    private readonly RabbitMQSetting setting;
    private readonly ILogger logger;

    private readonly NotificationHander notificationHander;

    public MessageServer(
        IOptions<RabbitMQSetting> options,
        ILogger<MessageServer> logger,
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
            logger.LogInformation("ConsumeRequestQueue runing");
            var factory = new ConnectionFactory()
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

            channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            channel.BasicConsume(queue: setting.RequestQueue, autoAck: false, consumer: consumer);

            consumer.Received += OnReceived;

            logger.LogInformation("ConsumeRequestQueue runded");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
    }

    private async Task OnReceived(object model, BasicDeliverEventArgs eventArgs)
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
}