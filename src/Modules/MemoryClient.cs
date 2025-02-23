using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Pursue.Extension.Cache
{
    public sealed class MemoryClient
    {
        private readonly IMemoryCache _cache;

        public MemoryClient(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 设置基础数据类型数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeoutSeconds">TTL 单位：秒</param>
        /// <returns></returns>
        public bool Set(string key, string value, int timeoutSeconds = 0)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                throw new NullReferenceException("key or value is null");
            if (timeoutSeconds <= 0)
                timeoutSeconds = 150;

            return _cache.Set(key, value, new DateTimeOffset(DateTime.Now.AddSeconds(timeoutSeconds))) != null;
        }

        /// <summary>
        /// 获取基础数据类型数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException("key is null");
            if (_cache.TryGetValue(key, out object outModel))
                return outModel;
            return default;
        }

        /// <summary>
        /// 获取基础数据类型数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">获取的值</param>
        /// <returns>获取成功 true ,获取失败 false</returns>
        public bool TryGetValue(string key, out string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                value = null;
                return false;
            }
            return _cache.TryGetValue(key, out value);
        }


        /// <summary>
        /// 设置对象数据
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeoutSeconds">TTL 单位：秒</param>
        /// <returns></returns>
        public bool Set<TEntity>(string key, TEntity value, int timeoutSeconds = 0) where TEntity : class
        {
            if (string.IsNullOrEmpty(key) || value == null)
                throw new NullReferenceException("key or value is null");
            if (timeoutSeconds <= 0)
                timeoutSeconds = 150;

            return _cache.Set(key, value, new DateTimeOffset(DateTime.Now.AddSeconds(timeoutSeconds))) != null;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TEntity Get<TEntity>(string key) where TEntity : class
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException("key or value is null");
            if (_cache.TryGetValue(key, out TEntity outModel))
                return outModel;
            return default;
        }

        /// <summary>
        /// 获取基础数据类型数据
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">获取的值</param>
        /// <returns>获取成功 true ,获取失败 false</returns>
        public bool TryGetValue<TEntity>(string key, out TEntity value) where TEntity : class
        {
            if (string.IsNullOrEmpty(key))
            {
                value = null;
                return false;
            }
            return _cache.TryGetValue(key, out value);
        }


        /// <summary>
        /// 插入字典类型值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public bool HashSet<TEntity>(string key, string dataKey, TEntity value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(dataKey) || value == null)
                throw new NullReferenceException("key or dataKey or value is null");
            var result = false;
            if (_cache.TryGetValue(key, out ConcurrentDictionary<string, object> valueResult))
            {
                if (valueResult.TryAdd(dataKey, value))
                {
                    _cache.Remove(key);
                    _cache.Set(key, valueResult, CacheOptions.MemoryCacheEntryOptions);
                    result = true;
                }
            }
            else
            {
                _cache.Set(key, new Dictionary<string, object> { { dataKey, value } }, CacheOptions.MemoryCacheEntryOptions);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 插入字典
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <param name="valueDics"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public bool HashSetAll<TEntity>(string key, IDictionary<string, TEntity> valueDics)
        {
            if (string.IsNullOrEmpty(key) || valueDics == null || valueDics.Count == 0)
                throw new NullReferenceException("key or keyValues is null");
            if (_cache.TryGetValue(key, out ConcurrentDictionary<string, TEntity> _))
            {
                _cache.Remove(key);
            }
            _cache.Set(key, valueDics, CacheOptions.MemoryCacheEntryOptions);
            return true;
        }

        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public TEntity HashGet<TEntity>(string key, string dataKey) where TEntity : class
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(dataKey))
                throw new NullReferenceException("key or dataKey is null");
            if (_cache.TryGetValue(key, out ConcurrentDictionary<string, object> valueDic))
            {
                if (valueDic.TryGetValue(dataKey, out object valueResult))
                {
                    return valueResult as TEntity;
                }
            }
            return default;
        }

        /// <summary>
        /// 获取整个字典
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public IDictionary<string, TEntity> HashGetAll<TEntity>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException("key is null");
            if (_cache.TryGetValue(key, out ConcurrentDictionary<string, TEntity> valueResult))
            {
                return valueResult;
            }
            return default;
        }


        /// <summary>
        /// 删除缓存的数据
        /// </summary>
        /// <param name="key">键</param>
        public bool Remove(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return false;
                _cache.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}