using MediatR;
using Microsoft.Extensions.Options;
using OWP.Core.NotificationRequests;
using OWP.MessageConsumer.Constants;
using OWP.MessageConsumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.MessageConsumer;

#nullable disable
public class Consumer
{
    private ConnectionFactory factory;
    private IConnection connection;
    private IModel channel;
    private readonly RabbitMQSetting setting;
    private readonly IMediator mediator;
    public Consumer(IOptions<RabbitMQSetting> options, IMediator mediator)
    {
        setting = options.Value;
        this.mediator = mediator;
        factory = new ConnectionFactory()
        {
            HostName = setting.Hosting,
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: setting.Exchange, type: "direct", durable: true, autoDelete: false);
        channel.QueueDeclare(queue: setting.Queue, durable: true, exclusive: false, autoDelete: false);
    }

    public void ReceiveMessageFromQueue()
    {
        try
        {
            foreach (var key in RoutingKey.Keys)
            {
                channel.QueueBind(queue: setting.Queue, exchange: setting.Exchange, routingKey: key);
            }

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += OnReceived;

            channel.BasicConsume(setting.Queue, autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
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