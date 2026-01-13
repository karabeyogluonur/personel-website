using Microsoft.Extensions.Caching.Memory;
using PW.Application.Common.Constants;
using PW.Application.Common.Interfaces;

namespace PW.Services.Caching;

public class MemoryCacheManager : ILocalCacheService
{
    private readonly IMemoryCache _memoryCache;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public MemoryCacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T> GetAsync<T>(string cacheKey)
    {
        _memoryCache.TryGetValue(cacheKey, out T value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? CacheDurations.Long
        };

        _memoryCache.Set(cacheKey, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_memoryCache.TryGetValue(cacheKey, out T cachedValue))
        {
            return cachedValue;
        }

        await _semaphore.WaitAsync();

        try
        {
            if (_memoryCache.TryGetValue(cacheKey, out T doubleCheckValue))
            {
                return doubleCheckValue;
            }

            var dbValue = await factory();

            if (dbValue != null)
            {
                await SetAsync(cacheKey, dbValue, expiration);
            }

            return dbValue;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
