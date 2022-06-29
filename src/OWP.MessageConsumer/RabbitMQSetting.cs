namespace OWP.MessageConsumer;

public class RabbitMQSetting
{
    public const string RabbitMQ = "RabbitMQ";
    public string Hosting { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string Exchange { get; set; }
    public string Queue { get; set; }
}

