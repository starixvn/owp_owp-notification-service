using MediatR;
using Microsoft.Extensions.Options;
using OWP.Core.Models;
using OWP.Core.NotificationRequests;
using OWP.MessageConsumer.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.MessageConsumer;

#nullable disable
public class Consumer
{
    private readonly RabbitMQSetting setting;
    private readonly IMediator mediator;
    //   private readonly ILogger logger;
    private const string Exchange = "SessionMessage";
    private const string QueueName = "Session";

    private ConnectionFactory factory;
    private IConnection connection;
    private IModel channel;
    public Consumer(IOptions<RabbitMQSetting> options, IMediator mediator)
    {
        setting = options.Value;
        this.mediator = mediator;
        this.CreateConnection();
        //  this.logger = logger;
    }

    private void CreateConnection()
    {
        try
        {
            factory = new ConnectionFactory()
            {
                HostName = setting.Hosting,
                VirtualHost = setting.VirtualHost,
                Port = setting.Port,
                Password = setting.Password,
                UserName = setting.UserName
            };
            connection = factory.CreateConnection();

            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: Exchange, type: "direct", durable: true, autoDelete: false);

            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
        }
        catch (Exception ex)
        {
            //  logger.Error(ex.Message);
            throw;
        }
    }

    public void ReceiveMessageFromQueue()
    {
        try
        {
            foreach (var key in RoutingKey.Keys)
            {
                channel.QueueBind(queue: QueueName, exchange: Exchange, routingKey: key);
            }

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += OnReceived;

            channel.BasicConsume(QueueName, autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            //  logger.Error(ex.Message);
            throw;
        }
    }

    private async void OnReceived(object model, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        var request = NotificationRequestFactory.GetNotificationRequest(eventArgs.RoutingKey, message);

        await mediator.Send(request);

    }
}