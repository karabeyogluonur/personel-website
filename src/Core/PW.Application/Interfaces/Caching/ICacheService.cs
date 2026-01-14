namespace PW.Application.Interfaces.Caching;

public interface ICacheService
{
   Task<T> GetAsync<T>(string cacheKey);
   Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null);
   Task RemoveAsync(string cacheKey);
   Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null);
}
