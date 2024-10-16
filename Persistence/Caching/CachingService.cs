using Contracts;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Persistence.Caching
{
    public sealed class CachingService : ICachingService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly Dictionary<string, bool> CachedKeys;
        private const string KeyOfCachedKeys = "CachedKeys";

        public CachingService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;

            var cachedKeys = _distributedCache.GetString(KeyOfCachedKeys);

            CachedKeys = cachedKeys is not null
                ? JsonSerializer.Deserialize<Dictionary<string, bool>>(cachedKeys)
                : new();

            _distributedCache.SetString(KeyOfCachedKeys, JsonSerializer.Serialize(CachedKeys));
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (cachedValue == null) return null;

            var value = JsonSerializer.Deserialize<T>(cachedValue, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default) where T : class
        {
            var cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue is not null) return cachedValue;

            cachedValue = await factory();

            await SetAsync<T>(key, cachedValue, cancellationToken);

            return cachedValue;
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            var jsonValue = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, jsonValue, cancellationToken);

            await AddKeyAsync(key, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellation = default)
        {
            await _distributedCache.RemoveAsync(key, cancellation);
            await RemoveKeyAsync(key, cancellation);
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellation = default)
        {
            var tasks = CachedKeys.Keys
                .Where(p => p.ToLower().StartsWith(prefix.ToLower()))
                .Select(key => RemoveAsync(key, cancellation));

            await Task.WhenAll(tasks);
        }

        private async Task AddKeyAsync(string key, CancellationToken cancellation = default)
        {
            CachedKeys.Add(key, true);
            await _distributedCache.RemoveAsync(KeyOfCachedKeys, cancellation);
            await _distributedCache.SetStringAsync(
                KeyOfCachedKeys,
                JsonSerializer.Serialize(CachedKeys),
                cancellation);
        }

        private async Task RemoveKeyAsync(string key, CancellationToken cancellation = default)
        {
            CachedKeys.Remove(key);
            await _distributedCache.RemoveAsync(KeyOfCachedKeys, cancellation);
            await _distributedCache.SetStringAsync(
                    KeyOfCachedKeys,
                    JsonSerializer.Serialize(CachedKeys),
                    cancellation);
        }
    }
}
