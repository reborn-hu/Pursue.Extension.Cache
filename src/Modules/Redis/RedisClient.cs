using CSRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pursue.Extension.Cache.DependencyInjection;
using System;

namespace Pursue.Extension.Cache
{
    public partial class RedisClient : CSRedisClient
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 创建redis访问类(支持单机或集群)
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connectionString">127.0.0.1[:6379],password=123456,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍</param>
        internal RedisClient(string connectionString) : base(null, Array.Empty<string>(), false, null, connectionString)
        {
            _logger = CacheClientDependencyInjection.ServiceProvider.GetService<ILogger<RedisClient>>();
        }

        /// <summary>
        /// 创建redis哨兵访问类(Redis Sentinel)
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connectionString"> mymaster,password=123456,poolsize=50,connectTimeout=200,ssl=false</param>
        /// <param name="sentinels">哨兵节点，如：ip1:26379、ip2:26379</param>
        /// <param name="readOnly">false: 只获取master节点进行读写操作,true: 只获取可用slave节点进行只读操作</param>
        internal RedisClient(string connectionString, string[] sentinels, bool readOnly = false) : base(null, sentinels, readOnly, null, connectionString)
        {
            _logger = CacheClientDependencyInjection.ServiceProvider.GetService<ILogger<RedisClient>>();
        }

        /// <summary>
        /// 创建redis哨兵访问类(Redis Sentinel) CSRedis.CSRedisClient
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connectionString">mymaster,password=123456,poolsize=50,connectTimeout=200,ssl=false</param>
        /// <param name="sentinels">哨兵节点，如：ip1:26379、ip2:26379</param>
        /// <param name="readOnly">false: 只获取master节点进行读写操作,true: 只获取可用slave节点进行只读操作</param>
        /// <param name="convert">哨兵主机转换规则</param>
        internal RedisClient(string connectionString, string[] sentinels, bool readOnly, SentinelMasterConverter convert) : base(null, sentinels, readOnly, convert, connectionString)
        {
            _logger = CacheClientDependencyInjection.ServiceProvider.GetService<ILogger<RedisClient>>();
        }

        /// <summary>
        /// 创建redis分区访问类，通过 KeyRule 对 key 进行分区，连接对应的 connectionString
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="NodeRule">按key分区规则，返回值格式：127.0.0.1:6379/13，默认方案(null)：取key哈希与节点数取模</param>
        /// <param name="connectionStrings">127.0.0.1[:6379],password=123456,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍</param>
        internal RedisClient(Func<string, string> NodeRule, params string[] connectionStrings) : base(NodeRule, null, readOnly: false, null, connectionStrings)
        {
            _logger = CacheClientDependencyInjection.ServiceProvider.GetService<ILogger<RedisClient>>();
        }
    }
}