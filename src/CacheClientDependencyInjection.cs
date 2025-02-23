using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Pursue.Extension.Cache.DependencyInjection
{
    public static class CacheClientDependencyInjection
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 装载Redis组件和本地缓存组件
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="options">缓存配置</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCacheClient(this IServiceCollection services, Action<CacheOptions> options)
        {
            if (options == null || options == null)
            {
                throw new ArgumentNullException(nameof(options), $"缓存配置不可为空！");
            }

            options.Invoke(new CacheOptions());

            services.AddSingleton<CacheOptions>();

            services.AddMemoryCache();
            services.AddSingleton<MemoryClient>();

            ServiceProvider = services.BuildServiceProvider();

            return services;
        }
    }
}