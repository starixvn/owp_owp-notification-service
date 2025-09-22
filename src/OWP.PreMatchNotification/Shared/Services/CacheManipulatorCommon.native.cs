using Fanex.Caching;
using Newtonsoft.Json;

namespace OWP.PreMatchNotification.Shared.Services;

public interface CacheManipulatorCommon
{
    Task<IList<T>> GetMultipleData<T>(string key);

    Task<T> GetSingleData<T>(string key, T defaultValue);

    Task RemoveData(string key);

    bool ContainKey(string key);

    Task Cache(string key, string value, TimeSpan expire);
}

public class CacheManipulatorCommonImp(ICache cache) : CacheManipulatorCommon
{
    public async Task<IList<T>> GetMultipleData<T>(string key)
    {
        var (_, value) = await cache.TryGetAsync<string>(key);
        var assignmentSnapshot = string.IsNullOrEmpty(value)
            ? []
            : JsonConvert.DeserializeObject<List<T>>(value);

        return assignmentSnapshot ?? [];
    }

    public async Task<T> GetSingleData<T>(string key, T defaultValue)
    {
        var (_, value) = await cache.TryGetAsync<string>(key);
        var assignmentSnapshot = string.IsNullOrEmpty(value)
            ? defaultValue
            : JsonConvert.DeserializeObject<T>(value);

        return assignmentSnapshot ?? defaultValue;
    }

    public async Task RemoveData(string key)
        => await cache.RemoveAsync(key);

    public async Task Cache(string key, string value, TimeSpan expire)
    {
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(expire);
        await cache.SetAsync(key, value, cachePolicy);
    }

    public bool ContainKey(string key)
        => cache.Contain(key);
}