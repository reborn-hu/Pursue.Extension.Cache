using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;

namespace Pursue.Extension.Cache
{
    public class CacheOptions
    {
        /// <summary>
        /// Key 前缀
        /// -- 默认: pursue:
        /// </summary>
        internal static string Prefix { get; private set; } = "pursue:";

        /// <summary>
        /// Redis配置
        /// </summary>
        internal static ConcurrentDictionary<string, RedisConnectionConfig> ConnectionSettings { get; private set; }

        /// <summary>
        /// MemoryCache配置
        /// </summary>
        internal static MemoryCacheEntryOptions MemoryCacheEntryOptions { get; set; } = new MemoryCacheEntryOptions()
        {
            Size = 100 * 1024,
            SlidingExpiration = TimeSpan.FromSeconds(150)
        };

        /// <summary>
        /// 配置自定义分布式缓存和本地缓存
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="redisSection"></param>
        /// <returns></returns>
        public CacheOptions UseCacheSettingsOptions(IConfiguration configuration, string redisSection = "Configuration:Redis")
        {
            var redisConfig = configuration.GetSection(redisSection).Get<RedisSettingsRoot>();

            Prefix = redisConfig.Prefix;
            ConnectionSettings = redisConfig.ConnectionSettings;

            return this;
        }
    }
}
