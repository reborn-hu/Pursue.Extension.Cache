using System.ComponentModel;

namespace Pursue.Extension.Cache
{
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
}
