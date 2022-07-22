using Fanex.Logging;
using MediatR;
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
            channel.QueueDeclare(queue: setting.RequestQueue, durable: true, autoDelete: false);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: setting.RequestQueue, autoAck: false, consumer: consumer);

            consumer.Received += OnReceived;
        }
        catch (Exception ex)
        {
            //log
        }
    }

    private async void OnReceived(object model, BasicDeliverEventArgs eventArgs)
    {
        var response = string.Empty;
        var props = eventArgs.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.Type = props.Type;

        try
        {
            var body = eventArgs.Body.ToArray();
            var requestMessage = Encoding.UTF8.GetString(body);

            response = await notificationHander.Handle(props.Type, requestMessage);
        }
        catch (Exception e)
        {
            //logger.Error(e.Message, e);
        }
        finally
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes);

            channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }
    }
}