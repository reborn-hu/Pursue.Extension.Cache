using Pursue.Extension.Cache.UnitTest.Init;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Pursue.Extension.Cache.UnitTest.Test
{
    [TestCaseOrderer(ordererTypeName: "Pursue.Extension.Cache.UnitTest.Init.Order.OrderByOrderer", ordererAssemblyName: "Pursue.Extension")]
    public class ConnectUnitTest : IClassFixture<AppResource>
    {
        private readonly AppResource _resource;
        private readonly ITestOutputHelper _output;

        private MemoryClient _memoryClient;
        private RedisClient _redisClient;

        public ConnectUnitTest(AppResource resource, ITestOutputHelper output)
        {
            _resource = resource;
            _output = output;

            _memoryClient = CacheFactory.GetMemoryClient();
            _redisClient = CacheFactory.GetRedisClient();
        }

        [Fact, OrderBy(1)]
        public void TestCreateClient()
        {
            _memoryClient = CacheFactory.GetMemoryClient();
            Assert.NotNull(_memoryClient);

            _redisClient = CacheFactory.GetRedisClient();
            Assert.NotNull(_redisClient);
        }

        [Fact, OrderBy(2)]
        public async Task TestSetAsync()
        {
            var result = await _redisClient.SetAsync("test111", "test111");
            Assert.True(result);
        }

        [Fact, OrderBy(3)]
        public async Task TestGetAsync()
        {
            var result = await _redisClient.GetAsync("test111");
            Assert.NotEmpty(result);
            Assert.Equal("test111", result);
        }

        [Fact, OrderBy(4)]
        public async Task TestSetHashAsync()
        {
            var result = await _redisClient.HSetAsync("test", "data_key", "value_1");
            Assert.True(result);
        }

        [Fact, OrderBy(5)]
        public async Task TestGetHashAsync()
        {
            var result = await _redisClient.HGetAsync("test", "data_key");
            Assert.NotEmpty(result);
            Assert.Equal("value_1", result);
        }
    }
}
