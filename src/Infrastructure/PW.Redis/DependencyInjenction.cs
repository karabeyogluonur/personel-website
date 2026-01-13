using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.Application.Common.Constants;
using PW.Application.Interfaces.Caching;
using PW.Redis.Services;
using StackExchange.Redis;

namespace PW.Redis;

public static class DependencyInjection
{
    public static void AddRedisServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Redis");
        var configOptions = ConfigurationOptions.Parse(connectionString);

        configOptions.AbortOnConnectFail = false;
        configOptions.ConnectTimeout = 5000;

        configOptions.SyncTimeout = 1000;
        configOptions.AsyncTimeout = 1000;

        configOptions.ReconnectRetryPolicy = new ExponentialRetry(deltaBackOffMilliseconds: 500, maxDeltaBackOffMilliseconds: 3000);

        configOptions.KeepAlive = 60;

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = configOptions;
            options.InstanceName = CacheKeys.Prefix;
        });

        builder.Services.AddSingleton<ICacheService, RedisCacheManager>();
    }
}
