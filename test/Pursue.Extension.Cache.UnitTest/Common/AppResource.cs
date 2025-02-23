using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pursue.Extension.Cache.DependencyInjection;
using System;

namespace Pursue.Extension.Cache.UnitTest
{
    public class AppResource : IDisposable
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public AppResource()
        {
            if (ServiceProvider == default)
            {
                var config = new ConfigurationManager().AddJsonFile($"./appsettings.Local.json", optional: true, reloadOnChange: true).Build();

                var service = new ServiceCollection();

                service.AddLogging();
                service.AddCacheClient(o => o.UseCacheSettingsOptions(config));

                ServiceProvider = service.BuildServiceProvider();
            }
        }

        public TEntity GetService<TEntity>()
        {
            return ServiceProvider.GetService<TEntity>();
        }


        public void Dispose()
        {
            ServiceProvider = default;
        }
    }
}
