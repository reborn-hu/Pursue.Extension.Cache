using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace Pursue.Extension.Cache
{
    public sealed class CacheConfigOptions
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
        public CacheConfigOptions UseCacheConfigOptions(IConfiguration configuration, string redisSection = "Configuration:Redis")
        {
            var redisConfig = configuration.GetSection(redisSection).Get<RedisSettingsRoot>();

            Prefix = redisConfig.Prefix;
            ConnectionSettings = redisConfig.ConnectionSettings;

            return this;
        }
    }

    public sealed class RedisSettingsRoot
    {
        /// <summary>
        /// Key 前缀
        /// <br>
        /// 默认：pursue
        /// </br>
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 连接参数字典
        /// </summary>
        public ConcurrentDictionary<string, RedisConnectionConfig> ConnectionSettings { get; set; } = new ConcurrentDictionary<string, RedisConnectionConfig>();
    }

    public sealed class RedisConnectionConfig
    {
        /// <summary>
        /// Redis连接类型
        /// -- single:单例 main:主从 sentinel:哨兵 cluster:集群
        /// -- 默认：single
        /// </summary>
        public RedisConnectType ConnectType { get; set; } = RedisConnectType.Single;

        /// <summary>
        /// 通讯协议
        /// </summary>
        public RedisProtocolType ProtocolType { get; set; } = RedisProtocolType.RESP2;

        /// <summary>
        /// 连接密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        public int Database { get; set; } = 0;

        /// <summary>
        /// 连接池大小 默认20
        /// </summary>
        public int PoolSize { get; set; } = 20;

        /// <summary>
        /// 连接超时
        /// -- 默认值：5000毫秒
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// 同步超时
        /// -- 默认值：10000毫秒
        /// </summary>
        public int SyncTimeout { get; set; } = 10000;

        /// <summary>
        /// 是否尝试集群模式，阿里云、腾讯云集群需要设置此选项为 false
        /// </summary>
        public bool TestCluster { get; set; } = true;

        /// <summary>
        /// 是否能执行危险指令
        /// -- 默认值：false
        /// </summary>
        public bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// 是否自动重连
        /// -- 默认值：false
        /// </summary>
        public bool AbortConnect { get; set; } = false;

        /// <summary>
        /// Redis 连接IP端口组
        /// </summary>
        public List<RedisEndpoint> Endpoints { get; set; } = new List<RedisEndpoint>();

        /// <summary>
        /// 哨兵主节点名称
        /// </summary>
        public string SentinelMain { get; set; } = "mymaster";

        /// <summary>
        /// 哨兵连接配置
        /// -- ConnectType = RedisConnectType.sentinel 时启用该连接配置
        /// </summary>
        public List<RedisEndpoint> Sentinels { get; set; } = new List<RedisEndpoint>();
    }

    public sealed class RedisEndpoint
    {
        /// <summary>
        /// 连接IP
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 连接Port
        /// </summary>
        public int Port { get; set; }
    }

    public enum RedisConnectType
    {
        [Description("集群")]
        Cluster,

        [Description("哨兵")]
        Sentinel,

        [Description("主从")]
        Main,

        [Description("单例")]
        Single
    }

    public enum RedisProtocolType
    {
        /// <summary>
        /// Redis Server 6.X之前老协议
        /// </summary>
        [Description("Redis Server 6.X之前老协议")]
        RESP2,

        /// <summary>
        /// Redis Server 6.X之后高级协议
        /// -- 基于新版协议有新特性支持
        /// -- 具体参考：https://raw.githubusercontent.com/redis/redis/6.0/00-RELEASENOTES
        /// </summary>
        [Description("Redis Server 6.X之后高级协议")]
        RESP3
    }
}
