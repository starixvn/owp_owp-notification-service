using Microsoft.Extensions.Options;
using OWP.MessageConsumer.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OWP.MessageConsumer;
public class Consumer
{
    private ConnectionFactory factory;
    private IConnection connection;
    private IModel channel;
    private readonly RabbitMQSetting setting;
    public Consumer(IOptions<RabbitMQSetting> options)
    {
        setting = options.Value;
    }

    public void ReceiveMessageFromQueue()
    {
        try
        {
            factory = new ConnectionFactory()
            {
                HostName = setting.Hosting,
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: setting.Exchange, type: "direct", durable: true);
            channel.QueueDeclare(queue: setting.Queue, durable: true);

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

    private async void OnReceived(object? model, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
    }
}