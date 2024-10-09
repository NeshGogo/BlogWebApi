using Contracts;
using Microsoft.Extensions.Caching.Distributed;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Persistence.Caching
{
    public sealed class CachingService : ICachingService
    {
        private readonly IDistributedCache _distributedCache;
        private static readonly Dictionary<string, bool> CachedKeys = new();

        public CachingService(IDistributedCache disposableCache) => _distributedCache = disposableCache;

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

            CachedKeys.Add(key, true);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellation = default)
        {
            await _distributedCache.RemoveAsync(key, cancellation);
            CachedKeys.Remove(key);
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellation = default)
        {
            var tasks = CachedKeys.Keys
                .Where(p => p.StartsWith(prefix))
                .Select(key => RemoveAsync(key, cancellation));

            await Task.WhenAll(tasks);
        }
    }
}
