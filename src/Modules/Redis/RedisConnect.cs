using System;
using System.Linq;
using System.Text;

namespace Pursue.Extension.Cache
{
    internal static class RedisConnect
    {
        /// <summary>
        /// 获取连接或者获取多个Redis实例
        /// </summary>
        /// <param name="connectionConfig"></param>
        /// <param name="db">DB</param>
        /// <returns></returns>
        internal static RedisClient CreateConnect(RedisConnectionConfig connectionConfig, int db = 0)
        {
            var sb = new StringBuilder();

            switch (connectionConfig.ConnectType)
            {
                // 单机、主从 
                case RedisConnectType.Single:
                case RedisConnectType.Main:
                    // 拉取一个配置节点, CsRedis特性:自监听该连接类型加载连接规则
                    var single = connectionConfig.Endpoints?.FirstOrDefault();

                    sb.AppendFormat($"{single.Host}:{single.Port}");
                    sb.AppendFormat(",password={0}", connectionConfig.Password);
                    sb.AppendFormat(",defaultDatabase={0}", db);
                    sb.AppendFormat(",prefix={0}", CacheOptions.Prefix);
                    sb.AppendFormat(",poolsize={0}", connectionConfig.PoolSize);
                    sb.AppendFormat(",connectTimeout={0}", connectionConfig.ConnectTimeout);
                    sb.AppendFormat(",syncTimeout={0}", connectionConfig.SyncTimeout);
                    sb.AppendFormat(",testcluster={0}", connectionConfig.TestCluster);

                    return new RedisClient(sb.ToString());

                // 集群
                case RedisConnectType.Cluster:
                    // 拉取一个配置节点, CsRedis特性:自监听该连接类型加载连接规则
                    var cluster = connectionConfig.Endpoints?.FirstOrDefault();

                    sb.AppendFormat($"{cluster.Host}:{cluster.Port}");
                    sb.AppendFormat(",password={0}", connectionConfig.Password);
                    sb.AppendFormat(",defaultDatabase=0");
                    sb.AppendFormat(",prefix={0}", CacheOptions.Prefix);
                    sb.AppendFormat(",poolsize={0}", connectionConfig.PoolSize);
                    sb.AppendFormat(",connectTimeout={0}", connectionConfig.ConnectTimeout);
                    sb.AppendFormat(",syncTimeout={0}", connectionConfig.SyncTimeout);
                    sb.AppendFormat(",testcluster={0}", connectionConfig.TestCluster);

                    return new RedisClient(sb.ToString());

                // 哨兵
                case RedisConnectType.Sentinel:

                    sb.AppendFormat(connectionConfig.SentinelMain);
                    sb.AppendFormat(",password={0}", connectionConfig.Password);
                    sb.AppendFormat(",defaultDatabase={0}", connectionConfig.Database);
                    sb.AppendFormat(",prefix={0}", CacheOptions.Prefix);
                    sb.AppendFormat(",poolsize={0}", connectionConfig.PoolSize);
                    sb.AppendFormat(",connectTimeout={0}", connectionConfig.ConnectTimeout);
                    sb.AppendFormat(",syncTimeout={0}", connectionConfig.SyncTimeout);
                    sb.AppendFormat(",testcluster={0}", connectionConfig.TestCluster);

                    var sentinelEndpoints = connectionConfig.Sentinels.Select(o => $"{o.Host}:{o.Port}").ToArray();

                    return new RedisClient(sb.ToString(), sentinelEndpoints);

                default:
                    throw new NullReferenceException("Config.EnumRedisConnectType, Redis connectType The configuration does not support this mode.");
            }
        }
    }
}