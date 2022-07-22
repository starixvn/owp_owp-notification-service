using Fanex.Logging;
using MediatR;
using Microsoft.Extensions.Options;
using OWP.Core.NotificationRequests;
using OWP.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.RabbitMQ;

public class MessageServer
{
    private ConnectionFactory factory;
    private IConnection connection;
    private IModel channel;

    private readonly RabbitMQSetting setting;
    private readonly IMediator mediator;
    private readonly ILogger logger;

    public MessageServer(IOptions<RabbitMQSetting> options, IMediator mediator, ILogger logger)
    {
        setting = options.Value;
        factory = new ConnectionFactory() { HostName = setting.Hosting };

        this.mediator = mediator;
        this.logger = logger;
    }

    public void ReceiveMessage()
    {
        try
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: setting.Queue, durable: true, autoDelete: false);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: setting.Queue, autoAck: false, consumer: consumer);

            consumer.Received += OnReceived;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
        }

    }

    private async void OnReceived(object model, BasicDeliverEventArgs eventArgs)
    {
        var props = eventArgs.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;


        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        var response = await HandleRequest(props.CorrelationId, message);

        var responseBytes = Encoding.UTF8.GetBytes(response);

        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

        channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);

    }

    private async Task<string> HandleRequest(string key, string value)
    {
        var request = NotificationRequestFactory.GetNotificationRequest(key, value);

        var result = await mediator.Send(request);

        return result;
    }

}