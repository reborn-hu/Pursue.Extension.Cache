using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pursue.Extension.Cache
{
    public partial class RedisClient
    {
        private const int QueueLockTimeKey = 5;
        private const string QueueThreadsNumKey = "threads:count";

        private static long _score = 0;
        private static long _timestampRecorde = 0;

        private readonly int _consumerNum = 1;
        private readonly object _lock = new object();

        /// <summary>
        /// Redis队列生产者
        /// </summary>
        /// <typeparam name="TEntity">入队数据类型</typeparam>
        /// <param name="options">参数对象</param>
        /// <returns></returns>
        public async Task PushAsync<TEntity>(RedisPushOptions<TEntity> options)
        {
            await Task.Run(() =>
            {
                var data = string.Empty;
                var queueKey = options.GetQueueKey();
                var queueHashKey = options.GetQueueHashKey();
                try
                {
                    // 处理数据序列化
                    if (typeof(TEntity) == typeof(string))
                        data = options.Body.ToString();
                    else
                        data = JsonConvert.SerializeObject(options.Body);

                    // 处理并发冲撞
                    var timestamp = options.Timestamp;
                    if (timestamp == _timestampRecorde)
                        _score++;
                    else
                        _score = 0;
                    _timestampRecorde = timestamp;

                    // 入队数据
                    using (var redisLock = TryLock($"{queueKey}:writeLock", QueueLockTimeKey))
                    {
                        try
                        {
                            XAdd(queueKey, id: $"{timestamp}-{_score}", ($"{timestamp}-{_score}", data));
                            HSetNxAsync(queueHashKey, queueKey, RedisExecStatusType.wait.ToString());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Redis写入失败, 队列:{}-{}, 数据:{}", options.ExchangeKey, options.QueueKey, data);
                            redisLock.Unlock();
                        }
                    }
                    _logger.LogInformation("写入成功, 队列:{}-{}", options.ExchangeKey, options.QueueKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "数据解析失败, 队列:{}-{}, 数据:{}", options.ExchangeKey, options.QueueKey, data);
                }
            });
        }

        /// <summary>
        /// Redis队列消费者
        /// </summary>
        /// <typeparam name="TEntity">出队数据类型</typeparam>
        /// <param name="options">参数对象</param>
        /// <param name="handler">业务执行委托</param>
        /// <param name="cancellationToken">取消信标</param>
        /// <returns></returns>
        public async Task ReceivedAsync<TEntity>(RedisReceiveOptions options, Action<TEntity, string, Action> handler, CancellationToken cancellationToken)
        {
            var action = new Action<RedisReceiveOptions, Action<TEntity, string, Action>, CancellationToken>((options, actionHandler, token) =>
            {
                var count = 1;
                var blockTime = 100;
                var runThreadCount = 0;
                var messageInitialId = "0-0";
                while (true)
                {
                    // 判断是否结束任务(根节点)
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // 获取线程配置数
                        var threadsCount = GetConsumerThreadsNumber(options.QueueKey);
                        // 空闲线程
                        var freeThreadsCount = threadsCount - runThreadCount < 0 ? 0 : threadsCount - runThreadCount;

                        // 判断当前运行线程数大于等于配置数和相减等于负数的不做处理
                        if (runThreadCount >= threadsCount && freeThreadsCount <= 0)
                        {
                            _logger.LogInformation("{}, 当前运行线程数已满, 当前正在执行线程数:{}, 剩余活动线程数:{}", options.QueueKey, runThreadCount, freeThreadsCount);
                            Thread.Sleep(1000);
                            continue;
                        }
                        else
                        {
                            // 扫描Hash表内需要处理的队列Key，如果hash表内无数据，证明没有可执行任务，等待2秒返回。
                            var queueKey = GetQueueHashKey(options.ExchangeKey);
                            if (string.IsNullOrEmpty(queueKey))
                            {
                                _logger.LogInformation("{}, 未获取到需要消费的Key, 当前正在执行线程数:{}, 剩余活动线程数:{}", $"{options.ExchangeKey}:{options.QueueKey}", runThreadCount, freeThreadsCount);
                                Thread.Sleep(2000);
                                continue;
                            }
                            else
                            {
                                runThreadCount++;
                                Task.Run(() =>
                                {
                                    var queueQueryKey = queueKey;
                                    var messageId = messageInitialId;
                                    var queueExecKey = options.GetQueueKey();
                                    var queueHashKey = options.GetQueueHashKey();
                                    _logger.LogInformation("队列查询键:{}, 启动执行任务键:{}, 消息编码:{}", queueQueryKey, queueExecKey, messageId);

                                    while (true)
                                    {
                                        try
                                        {
                                            // 循环出队Redis队列数据
                                            var result = XRead(count, blockTime, (queueQueryKey, messageId));
                                            if (result != null)
                                            {
                                                // 获取报文文档数据
                                                var items = result.FirstOrDefault().data.FirstOrDefault().items;
                                                if (items != null)
                                                {
                                                    // 赋值消息Id，递归下一条消息的游标
                                                    messageId = items.First();
                                                    _logger.LogInformation("任务执行, 任务:{}, 消息编码:{}", queueExecKey, messageId);
                                                    try
                                                    {
                                                        var jsonModel = JsonConvert.DeserializeObject<TEntity>(items.Last());
                                                        // 执行业务逻辑
                                                        handler(jsonModel, items.First(), () =>
                                                        {
                                                            var id = XDel(queueQueryKey, messageId);
                                                            _logger.LogInformation("执行完毕, 任务:{}, 消息编码:{}", queueExecKey, messageId);
                                                        });
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _logger.LogError(ex, "执行失败, 消息体解析失败, 任务:{}, 消息编码:{}", queueExecKey, messageId);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // 移除Hash中已经处理完毕的Key
                                                HDel(queueHashKey, queueQueryKey);
                                                break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "执行异常, 任务:{}", queueExecKey);
                                            continue;
                                        }
                                    }
                                }, cancellationToken)
                                .ContinueWith(task =>
                                {
                                    if (!cancellationToken.IsCancellationRequested)
                                    {
                                        if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                                        {
                                            // 上锁判断执行完毕后释放线程数
                                            lock (_lock)
                                            {
                                                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                                                {
                                                    runThreadCount--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.LogError(task.Exception, "子任务Id:{}, 完成状态:{}, 故障状态:{}, 取消状态:{}", task.Id, task.IsCompleted, task.IsFaulted, task.IsCanceled);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogWarning("子服务已退出！");
                                        return;
                                    }

                                }, token);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            });

            await Task.Run(() =>
            {
                action(options, handler, cancellationToken);
            }, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// 队列Hash表
        /// </summary>
        /// <param name="exchangeKey">队列Code</param>
        /// <returns></returns>
        private string GetQueueHashKey(string exchangeKey)
        {
            var hashKey = $"{exchangeKey}:hash";
            using (var redisLock = Lock($"{hashKey}:readlock", QueueLockTimeKey))
            {
                try
                {
                    var hash = HGetAll(hashKey);

                    var queueKey = hash.FirstOrDefault(o => o.Value.Equals(RedisExecStatusType.wait.ToString())).Key;

                    HSet(hashKey, queueKey, RedisExecStatusType.executing.ToString());

                    return queueKey;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 获取消费者线程数配置
        /// </summary>
        /// <param name="queueKey">队列Code</param>
        /// <returns></returns>
        private int GetConsumerThreadsNumber(string queueKey)
        {
            try
            {
                if (string.IsNullOrEmpty(queueKey))
                {
                    return _consumerNum;
                }
                else
                {
                    var threadsNum = HGet<string>(QueueThreadsNumKey, queueKey);
                    if (!string.IsNullOrEmpty(threadsNum) && int.TryParse(threadsNum, out int num))
                    {
                        return num;
                    }
                    else
                    {
                        HSet(QueueThreadsNumKey, queueKey, _consumerNum);
                        return _consumerNum;
                    }
                }
            }
            catch
            {
                return _consumerNum;
            }
        }
    }
}