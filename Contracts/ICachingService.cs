namespace Contracts;

public interface ICachingService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default) where T : class;

    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;

    Task RemoveAsync(string key, CancellationToken cancellation = default);

    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellation = default);
}
