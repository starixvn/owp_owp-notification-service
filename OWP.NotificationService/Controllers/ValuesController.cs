using Fanex.Caching;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace OWP.NotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICache cache;

        public ValuesController(ICache cache, IConfiguration configuration = null)
        {
            this.cache = cache;
            this.configuration = configuration;
        }

        private readonly IConfiguration configuration;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var connectionString = configuration.GetSection("RedisConnect:ConnectionString").Value;
            var options = ConfigurationOptions.Parse(connectionString);
            var redis = ConnectionMultiplexer.Connect(options);

            var endPoints = redis.GetEndPoints();

            var appName = configuration.GetSection("fanex.caching.redis:ApplicationName").Value;
            var keyPrefix = $"{{{appName}}}:{{";

            var keys = endPoints.Select(e => redis.GetServer(e).Keys(11, pattern: $"{{{appName}}}:*"))
                .SelectMany(k => k)
                .Select(k => k.ToString().Replace(keyPrefix, string.Empty))
                .Select(k => k.Remove(k.Length - 1))
                .Distinct()
                .OrderBy(k => k)
                .ToArray();

            return Ok(keys);

        }

        [HttpGet("{cacheKey}")]
        public async Task<IActionResult> DeleteCache(string cacheKey)
        {
            var value = cache.TryGet<string>(cacheKey, out string ab);
            //  cache.Remove(cacheKey);

            return Ok();
        }
    }
}
