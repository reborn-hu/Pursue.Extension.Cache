using System;

namespace Pursue.Extension.Cache
{
    /// <summary>
    /// 数据循环总线
    /// </summary>
    /// <typeparam name="TEntity">需要获取或设置的数据类型</typeparam>
    public sealed class DataCircleBus<TEntity>
    {
        /// <summary>
        /// 获取本地缓存数据
        /// </summary>
        public Func<TEntity> GetLocalData { get; set; } = default;

        /// <summary>
        /// 获取Redis数据
        /// </summary>
        public Func<TEntity> GetRedisData { get; set; } = default;

        /// <summary>
        /// 获取DB数据
        /// </summary>
        public Func<TEntity> GetDatabaseData { get; set; } = default;

        /// <summary>
        /// 设置本地缓存数据
        /// </summary>
        public Action<TEntity> SetLocalData { get; set; } = default;

        /// <summary>
        /// 设置Redis缓存数据
        /// </summary>
        public Action<TEntity> SetRedisData { get; set; } = default;

        /// <summary>
        /// 获取数据入口，优先获取本地缓存
        /// </summary>
        /// <param name="input">数据循环总线委托</param>
        /// <returns></returns>
        public TEntity GetData(DataCircleBus<TEntity> input)
        {
            if (input.GetLocalData != default)
            {
                // 获取本地缓存
                var result = input.GetLocalData();
                if (result != null)
                {
                    return result;
                }
            }
            return QueryRedisData(input);
        }

        /// <summary>
        /// Redis查询数据设置本地缓存，如果没数据查询数据库
        /// </summary>
        /// <param name="input">数据循环总线委托</param>
        /// <returns></returns>
        private static TEntity QueryRedisData(DataCircleBus<TEntity> input)
        {
            if (input.GetRedisData != default)
            {
                // 获取redis数据
                var result = input.GetRedisData();
                if (result != null)
                {
                    // 设置本地缓存数据
                    if (input.SetLocalData != default)
                    {
                        input.SetLocalData.Invoke(result);
                    }
                    return result;
                }
            }
            return QueryDatabaseData(input);
        }

        /// <summary>
        /// 从数据库中获取数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static TEntity QueryDatabaseData(DataCircleBus<TEntity> input)
        {
            if (input.GetDatabaseData != default)
            {
                // 获取数据库数据
                var result = input.GetDatabaseData();
                if (result != null)
                {
                    // 设置redis数据
                    if (input.SetRedisData != default)
                    {
                        input.SetRedisData.Invoke(result);
                    }

                    // 设置本地缓存数据
                    if (input.SetLocalData != default)
                        input.SetLocalData.Invoke(result);
                }
                return result;
            }
            return default;
        }
    }
}