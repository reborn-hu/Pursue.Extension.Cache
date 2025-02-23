namespace Pursue.Extension.Cache
{
    public sealed class RedisReceiveOptions
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
        /// 是否开启自动删除队列内数据。
        /// <para>默认设置: true , 需要调用回调中的 autoRemove() 方法。</para>
        /// </summary>
        public bool AutoRemove { get; private set; } = true;

        /// <summary>
        /// 创建Redis队列消费配置
        /// </summary>
        /// <param name="exchangeKey">
        /// Redis队列名称，对应处理任务作业类型。系统类型+场景
        /// <para>真实值 = Queue:<paramref name="exchangeKey"/> . 如: 场景 = Pay; 业务类型 = Order; 真实值 = Queue:PayOrder;</para>
        /// </param>
        /// <param name="queueKey">
        /// Redis执行队列名称，报文存储的真实队列名称。
        /// <para>真实值 = Queue:<paramref name="exchangeKey"/>:<paramref name="queueKey"/> . 如: exchangeKey = PayOrder; queueKey = SessionId; 真实值 = Queue:PayOrder:SessionId;</para>
        /// </param>
        /// <param name="autoRemove">
        /// 是否开启自动删除队列内数据。
        /// <para>默认设置: true , 需要调用回调中的 autoRemove() 方法。</para>
        /// </param>
        public RedisReceiveOptions(string exchangeKey, string queueKey, bool autoRemove = true)
        {
            ExchangeKey = exchangeKey;
            QueueKey = queueKey;
            AutoRemove = autoRemove;
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