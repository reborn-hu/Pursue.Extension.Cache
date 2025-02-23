namespace Pursue.Extension.Cache
{
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
}
