namespace OWP.Core.Models;

public class RedisSetting
{
    public const string RedisConnectKey = "RedisConnect";
    public string ConnectionString { get; set; }
    public string AppName { get; set; }
}