using System.Collections.Generic;

namespace Pursue.Extension.Cache.Demo.src
{
    public class DataCircleBusDemo
    {
        private readonly MemoryClient _memory;
        private readonly RedisClient _redis;
        //private readonly IRepository<XXX> _repository;

        public DataCircleBusDemo(/*IRepositoryFactory repository*/)
        {
            _memory = CacheFactory.GetMemoryClient();
            _redis = CacheFactory.GetRedisClient();
            //_repository = repository.GetWriteRepository<XXX>(DbBusinessType.Base);
        }

        public List<string> GetCircleBusData()
        {
            var dataCircleBus = new DataCircleBus<List<string>>
            {
                GetLocalData = () =>
                {
                    // _memory.Get("");
                    // 实现获取本地缓存数据逻辑
                    return new List<string> { "" };
                },
                GetRedisData = () =>
                {
                    // _redis.Get("");
                    // 实现获取redis数据逻辑
                    return new List<string> { "" };
                },
                GetDatabaseData = () =>
                {
                    //_repository.AsQueryable().Where().....
                    // 实现获取数据库数据逻辑
                    return new List<string> { "" };
                },
                SetLocalData = (value) =>
                {
                    // _memory.Set("");
                    // 实现写本地数据逻辑
                },
                SetRedisData = (value) =>
                {
                    // _redis.Set("");
                    // 实现写redis逻辑
                }
            };

            // 获取最终数据
            return dataCircleBus.GetData(dataCircleBus);
        }
    }
}
