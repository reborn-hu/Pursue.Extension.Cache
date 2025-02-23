using Microsoft.Extensions.DependencyInjection;
using Pursue.Extension.Cache.DependencyInjection;
using System.Collections.Concurrent;

namespace Pursue.Extension.Cache
{
    public static class CacheFactory
    {
        private static MemoryClient _memoryClient;
        private static ConcurrentDictionary<string, RedisClient> _instances = new ConcurrentDictionary<string, RedisClient>();

        /// <summary>
        /// 获取MemoryCache操作客户端
        /// </summary>
        /// <returns></returns>
        public static MemoryClient GetMemoryClient()
        {
            if (_memoryClient == default)
            {
                _memoryClient = CacheClientDependencyInjection.ServiceProvider.GetService<MemoryClient>();
            }
            return _memoryClient ?? default;
        }

        /// <summary>
        /// 获取其它节点配置
        /// </summary>
        /// <param name="nodeSection">配置文件中不同实例名称</param>
        /// <returns></returns>
        public static RedisClient GetRedisClient(string nodeSection = "Default")
        {
            var connectionSettings = CacheOptions.ConnectionSettings[nodeSection];
            if (connectionSettings != null && connectionSettings.Endpoints.Count > 0)
            {
                var db = (connectionSettings.Database <= 0 || connectionSettings.Database >= 254) ? 0 : connectionSettings.Database;
                var key = $"{nodeSection}_{db}";

                if (_instances.TryGetValue(key, out RedisClient redisClient))
                {
                    return redisClient;
                }
                else
                {
                    var client = RedisConnect.CreateConnect(connectionSettings, db);

                    _instances.TryAdd(key, client);

                    return client;
                }
            }
            return default;
        }
    }
}
