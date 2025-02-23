using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Pursue.Extension.Cache.Demo.src
{
    public class RedisQueueDemo : BackgroundService
    {

        private readonly ILogger _logger;
        private readonly RedisClient _redis;

        public RedisQueueDemo(ILogger<RedisQueueDemo> logger)
        {
            _logger = logger;

            _redis = CacheFactory.GetRedisClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (int i = 0; i < 10; i++)
            {
                await _redis.PushAsync(new RedisPushOptions<DemoValue>("Test", "Demo", new DemoValue { Key = i.ToString(), Value = $"test-{i}" }));
            }

            await _redis.ReceivedAsync<DemoValue>(new RedisReceiveOptions("Test", "Demo", false), (data, msg, ack) =>
            {

                _logger.LogInformation(data.Value);
                _logger.LogInformation(msg);
                ack();

                Thread.Sleep(1000);
            }, stoppingToken);
        }
    }

    public class DemoValue
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
