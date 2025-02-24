# Pursue.Extension.Cache

Pursue.Extension.Cache 是Pursue系列缓存管理库，支持.NET6+，提供了简单易用的接口和实现，帮助开发者更高效地管理应用程序中的缓存。

## 功能特性

- **多种缓存提供程序**: 支持内存缓存、分布式缓存、二级缓存等多种缓存提供程序。
- **灵活的配置**: 提供灵活的配置选项，满足不同应用场景的需求。
- **易于扩展**: 设计良好的接口，方便开发者扩展和定制缓存功能。

## 安装

可以通过 NuGet 包管理器安装此库：

```sh
dotnet add package Pursue.Extension.Cache
```

## 快速开始
以下是一个简单的使用示例：

```csharp

// 控制台程序注入服务
if (ServiceProvider == default)
{
    var config = new ConfigurationManager()
        .AddJsonFile($"./appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    var service = new ServiceCollection();

    service.AddLogging();
    service.AddCacheClient(option =>{
        option.UseCacheSettingsOptions(config)
    });
    
    ServiceProvider = service.BuildServiceProvider();
}

```

```csharp

// web项目注入服务
builder.Services.AddCacheClient(options =>
{
    options.UseCacheSettingsOptions(builder.Configuration);
});


```

```csharp

// 具体使用
public class Foo 
{
    private MemoryClient _memory;
    private RedisClient _redis;

    public Foo()
    {
        _memory = CacheFactory.GetMemoryClient();
        _redis = CacheFactory.GetRedisClient();
    }
    
    public async Task TestCacheAsync()
    {
        var result =  _memory.Set("test", "test");
        var result = await _redis.SetAsync("test", "test");
    }
}


```


## Redis队列使用示例

以下是一个基于Redis Stream（需要redis版本6+）的简单使用示例：

```csharp

// web项目注入服务
builder.Services.AddCacheClient(options =>
{
    options.UseCacheSettingsOptions(builder.Configuration);
});

// 简单示例
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
        // 生产
        for (int i = 0; i < 10; i++)
        {
            var model = new DemoValue { Key = i.ToString(), Value = $"test-{i}" };
            await _redis.PushAsync(new RedisPushOptions<DemoValue>("Test", "Demo", model));
        }

        // 消费
        var option = new RedisReceiveOptions("Test", "Demo", false);
        await _redis.ReceivedAsync<DemoValue>(option, (data, msg, ack) =>
        {
            _logger.LogInformation(data.Value);
            _logger.LogInformation(msg);
            
            // 手动Ack
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

```


## 二级缓存使用示例

以下是一个基于Redis、本地缓存的二级缓存简单使用示例：

```csharp
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


```

