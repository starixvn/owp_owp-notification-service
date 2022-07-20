using Fanex.Logging;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OWP.Core.NotificationRequests;
using OWP.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.RabbitMQ;

public class MessageServer
{
    private const string QueueName = "Message";

    private ConnectionFactory factory;
    private IConnection connection;
    private IModel channel;

    private readonly RabbitMQSetting setting;
    private readonly IMediator mediator;

    public MessageServer(IOptions<RabbitMQSetting> options, IMediator mediator, ILogger logger)
    {
        setting = options.Value;

        this.mediator = mediator;

        factory = new ConnectionFactory() { HostName = setting.Hosting };
    }

    public void ReceiveMessage()
    {
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        consumer.Received += OnReceived;

    }

    private async void OnReceived(object model, BasicDeliverEventArgs eventArgs)
    {
        var response = String.Empty;
        var props = eventArgs.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            response = await HandleRequest(props.CorrelationId, message);
        }
        catch (Exception e)
        {
            //
            throw;
        }
        finally
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);

            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

            channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }
    }

    private async Task<string> HandleRequest(string key, string value)
    {
        var request = NotificationRequestFactory.GetNotificationRequest(key, value);

        var result = await mediator.Send(request) as List<object>;

        var json = JsonConvert.SerializeObject(result);

        return json;
    }

}