using System;

namespace Pursue.Extension.Cache
{
    public sealed class RedisPushOptions<TEntity>
    {
        /// <summary>
        /// Redis队列交换空间。业务场景+业务类型
        /// <para>真实值 = Queue:ExchangeKey . 如：场景 = Pay; 业务类型 = Order; 真实值 = Queue:PayOrder;</para>
        /// </summary>
        public string ExchangeKey { get; private set; }

        /// <summary>
        /// Redis队列，报文存储的真实队列名称。
        /// <para>真实值 = Queue:ExchangeKey:QueueKey . 如：ExchangeKey = PayOrder; QueueKey = SessionId; 真实值 = Queue:PayOrder:SessionId;</para>
        /// </summary>
        public string QueueKey { get; private set; }

        /// <summary>
        /// Redis执行队列Hash存储表名称，轮询扫描的HashKey。
        /// <para>真实值 = Queue:ExchangeKey:QueueHashKey . 如：ExchangeKey = PayOrder; QueueHashKey = Hash; 真实值 = Queue:PayOrder:Hash;</para>
        /// </summary>
        private string QueueHashKey { get; set; } = "hash";

        /// <summary>
        /// 数据操作时间戳，防并发。
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// 数据主体。
        /// </summary>
        public TEntity Body { get; private set; }

        /// <summary>
        /// 创建Redis队列入队配置
        /// </summary>
        /// <param name="exchangeKey">Redis队列交换空间</param>
        /// <param name="queueKey">Redis队列</param>
        /// <param name="body">数据主体</param>
        public RedisPushOptions(string exchangeKey, string queueKey, TEntity body)
        {
            ExchangeKey = exchangeKey;
            QueueKey = queueKey;
            Body = body;
            Timestamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }


        public string GetQueueKey()
        {
            return $"{ExchangeKey}:{QueueKey}";
        }

        public string GetQueueHashKey()
        {
            return $"{ExchangeKey}:{QueueHashKey}";
        }
    }
}