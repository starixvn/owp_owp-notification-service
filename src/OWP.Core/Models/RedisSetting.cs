namespace OWP.Core.Models;

public class RedisSetting
{
    public static string RedisConnectKey = "RedisConnect";
    public string ConnectionString { get; set; }
    public string AppName { get; set; }
}