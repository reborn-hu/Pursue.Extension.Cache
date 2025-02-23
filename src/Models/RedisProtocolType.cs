using System.ComponentModel;

namespace Pursue.Extension.Cache
{
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