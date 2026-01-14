using Microsoft.Extensions.Caching.Distributed;
using PW.Application.Common.Constants;
using PW.Application.Common.Interfaces;
using PW.Application.Extensions;
using PW.Application.Interfaces.Caching;

namespace PW.Redis.Services;

public class RedisCacheManager : ICacheService
{
   private readonly IDistributedCache _distributedCache;
   private readonly ILocalCacheService _localCache;
   private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

   public RedisCacheManager(IDistributedCache distributedCache, ILocalCacheService localCache)
   {
      _distributedCache = distributedCache;
      _localCache = localCache;
   }

   public async Task<T> GetAsync<T>(string cacheKey)
   {
      T memoryValue = await _localCache.GetAsync<T>(cacheKey);

      if (memoryValue != null && !EqualityComparer<T>.Default.Equals(memoryValue, default(T)))
      {
         return memoryValue;
      }

      byte[]? redisBytes = await _distributedCache.GetAsync(cacheKey);

      if (redisBytes is not null)
      {
         var redisValue = redisBytes.FromByteArray<T>();

         await _localCache.SetAsync(cacheKey, redisValue, CacheDurations.Long);

         return redisValue;
      }

      return default;
   }

   public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null)
   {
      T cachedValue = await _localCache.GetAsync<T>(cacheKey);

      if (cachedValue != null && !EqualityComparer<T>.Default.Equals(cachedValue, default(T)))
         return cachedValue;


      await _semaphoreSlim.WaitAsync();
      try
      {
         var doubleCheck = await GetAsync<T>(cacheKey);

         if (doubleCheck != null && !EqualityComparer<T>.Default.Equals(doubleCheck, default(T)))
            return doubleCheck;

         var dbValue = await factory();

         if (dbValue != null)
            await SetAsync(cacheKey, dbValue, expiration);

         return dbValue;
      }
      finally
      {
         _semaphoreSlim.Release();
      }
   }

   public async Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null)
   {
      var duration = expiration ?? CacheDurations.Long;

      await _localCache.SetAsync(cacheKey, value, duration);

      byte[]? bytes = value.ToByteArray();
      await _distributedCache.SetAsync(cacheKey, bytes, new DistributedCacheEntryOptions
      {
         AbsoluteExpirationRelativeToNow = duration
      });
   }

   public async Task RemoveAsync(string cacheKey)
   {
      await _localCache.RemoveAsync(cacheKey);
      await _distributedCache.RemoveAsync(cacheKey);
   }
}
