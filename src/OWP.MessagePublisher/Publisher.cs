using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OWP.Core.Models;
using RabbitMQ.Client;
using System.Text;

namespace OWP.MessagePublisher;

public class Publisher
{
    private readonly RabbitMQSetting setting;
    private const string Exchange = "MessageNotification";
    private const string QueueName = "Message";
    public Publisher(IOptions<RabbitMQSetting> options)
    {
        setting = options.Value;
    }

    public void PushMessage(object message, string routingKey)
    {
        var factory = new ConnectionFactory
        {
            HostName = setting.Hosting,
            VirtualHost = setting.VirtualHost,
            Port = setting.Port,
            Password = setting.Password,
            UserName = setting.UserName
        };

        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: Exchange, type: "direct", durable: true, autoDelete: false);
        channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: Exchange, routingKey: routingKey, body: body);
    }
}