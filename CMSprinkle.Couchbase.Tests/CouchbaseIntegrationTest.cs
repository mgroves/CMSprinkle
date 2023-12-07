using CMSprinkle.Infrastructure;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Testcontainers.Couchbase;

namespace CMSprinkle.Couchbase.Tests;

public abstract class CouchbaseIntegrationTest
{
    protected static AsyncLazy<CouchbaseContainer> _couchbaseContainer = new(async () =>
    {
        var couchbaseContainer = new CouchbaseBuilder()
            .WithImage("couchbase:enterprise-7.2.2")
            .Build();

        await couchbaseContainer.StartAsync();

        return couchbaseContainer;
    });

    protected ServiceCollection? _serviceCollection;
    protected string _scopeName;
    protected Random _random;
    protected string _bucketName;

    [SetUp]
    public virtual async Task Setup()
    {
        var couchbaseContainer = await _couchbaseContainer;
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddCMSprinkle();
        _serviceCollection.AddHttpContextAccessor();
        _serviceCollection.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Debug);
        });
        _serviceCollection.AddCouchbase(async options =>
        {
            options.ConnectionString = couchbaseContainer.GetConnectionString();
            options.UserName = CouchbaseBuilder.DefaultUsername;
            options.Password = CouchbaseBuilder.DefaultPassword;
        });
        _random = new Random();
        _scopeName = "_default";
        _bucketName = couchbaseContainer.Buckets.First().Name;
    }

    [TearDown]
    public async Task Teardown()
    {
        var builder = _serviceCollection.BuildServiceProvider();
        var lifetime = builder.GetRequiredService<ICouchbaseLifetimeService>();
        await lifetime.CloseAsync();
    }
}