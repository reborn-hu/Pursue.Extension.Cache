using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pursue.Extension.Cache.Demo.src;
using Pursue.Extension.Cache.DependencyInjection;
using System;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
try
{
    var builder = Host.CreateDefaultBuilder();

    builder.ConfigureServices(service =>
    {
        var config = new ConfigurationManager().AddJsonFile($"./appsettings.Local.json", optional: true, reloadOnChange: true).Build();

        service.AddLogging(option =>
        {
            option.AddConsole();
            option.SetMinimumLevel(LogLevel.Trace);
        });

        service.AddCacheClient(option =>
        {
            option.UseCacheSettingsOptions(config);
        });

        service.AddHostedService<RedisQueueDemo>();
    });

    var app = builder.Build();

    app.Run();
}
catch (Exception)
{

    throw;
}

