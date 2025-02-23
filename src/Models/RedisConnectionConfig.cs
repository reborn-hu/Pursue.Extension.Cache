using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Pursue.Extension.Cache
{
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
}