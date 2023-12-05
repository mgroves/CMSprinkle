using CMSprinkle.Couchbase.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using CMSprinkle.Data;

namespace CMSprinkle.Couchbase.Tests.CouchbaseCMSprinkleDataServiceTests;

[TestFixture]
public class InitializeDatabaseTests : CouchbaseIntegrationTest
{
    [Test]
    public async Task Collection_gets_created_by_default()
    {
        // arrange
        var collectionName = $"contentCollection_{_random.Next(100000)}";
        _serviceCollection.AddCMSprinkleCouchbase(_bucketName, _scopeName, collectionName);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        var bucketProvider = serviceProvider.GetService<ICmsBucketProvider>();

        // act
        await dataService.InitializeDatabase();

        // assert
        Assert.That(await bucketProvider.CollectionExists(_scopeName, collectionName), Is.True);
    }

    [Test]
    public async Task Collection_set_to_not_created()
    {
        // arrange
        var collectionName = $"contentCollection_{_random.Next(100000)}";
        _serviceCollection.AddCMSprinkleCouchbase(_bucketName, _scopeName, collectionName, createCollectionIfNecessary: false);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        var bucketProvider = serviceProvider.GetService<ICmsBucketProvider>();

        // act
        await dataService.InitializeDatabase();

        // assert
        Assert.That(await bucketProvider.CollectionExists(_scopeName, collectionName), Is.False);
    }

    [Test]
    public async Task Empty_ContentIndex_document_created()
    {
        // arrange
        var collectionName = $"contentCollection_{_random.Next(100000)}";
        _serviceCollection.AddCMSprinkleCouchbase(_bucketName, _scopeName, collectionName);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        var bucketProvider = serviceProvider.GetService<ICmsBucketProvider>();

        // act
        await dataService.InitializeDatabase();

        // assert
        Assert.That(await bucketProvider.DocumentExists(_scopeName, collectionName, "ContentIndex"), Is.True);
    }
}