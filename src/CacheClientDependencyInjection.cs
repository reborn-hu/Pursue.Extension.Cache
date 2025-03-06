using Microsoft.Extensions.DependencyInjection;
using Pursue.Extension.Cache;
using System;

namespace Pursue.Extension.DependencyInjection
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
        public static IServiceCollection AddCacheClient(this IServiceCollection services, Action<CacheConfigOptions> options)
        {
            if (options == null || options == null)
            {
                throw new ArgumentNullException(nameof(options), $"缓存配置不可为空！");
            }

            options.Invoke(new CacheConfigOptions());

            services.AddSingleton<CacheConfigOptions>();

            services.AddMemoryCache();
            services.AddSingleton<MemoryClient>();

            ServiceProvider = services.BuildServiceProvider();

            return services;
        }
    }
}