using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.Application.Common.Constants;
using PW.Application.Interfaces.Caching;
using PW.Caching.Services;
using StackExchange.Redis;

namespace PW.Caching;

public static class DependencyInjection
{
   public static void AddCachingServices(this IHostApplicationBuilder builder)
   {
      builder.Services.AddMemoryCache();
      builder.Services.AddSingleton<ILocalCacheService, MemoryCacheManager>();

      string? redisConnectionString = builder.Configuration.GetConnectionString("Redis");

      if (!string.IsNullOrWhiteSpace(redisConnectionString))
      {

         var configOptions = ConfigurationOptions.Parse(redisConnectionString);

         configOptions.AbortOnConnectFail = false;
         configOptions.ConnectTimeout = 5000;
         configOptions.SyncTimeout = 1000;
         configOptions.AsyncTimeout = 1000;
         configOptions.KeepAlive = 60;
         configOptions.ReconnectRetryPolicy = new ExponentialRetry(deltaBackOffMilliseconds: 500, maxDeltaBackOffMilliseconds: 3000);

         builder.Services.AddStackExchangeRedisCache(options =>
         {
            options.ConfigurationOptions = configOptions;
            options.InstanceName = CacheKeys.Prefix;
         });

         builder.Services.AddSingleton<ICacheService, RedisCacheManager>();
      }
      else
         builder.Services.AddSingleton<ICacheService>(provider => provider.GetRequiredService<ILocalCacheService>());
   }
}
