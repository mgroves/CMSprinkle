using CMSprinkle.Data;
using Microsoft.Extensions.DependencyInjection;
using CMSprinkle.ViewModels;

namespace CMSprinkle.Couchbase.Tests.CouchbaseCMSprinkleDataServiceTests;

[TestFixture]
public class GetTests : CouchbaseIntegrationTest
{
    private ICMSprinkleDataService _dataService;

    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();

        var collectionName = $"coll_{_random.Next(100000)}";
        _serviceCollection.AddCMSprinkleCouchbase(_bucketName, _scopeName, collectionName);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        await _dataService.InitializeDatabase();
    }

    [Test]
    public async Task Get_returns_result_for_a_key()
    {
        // arrange
        var existingKey = new Bogus.DataSets.Hacker().Noun() + _random.Next(10000);
        var existingContent = new Bogus.DataSets.Lorem().Paragraphs(3);

        await _dataService.AddNew(new AddContentSubmitModel
        {
            Key = existingKey,
            Content = existingContent
        });

        // act
        var result = await _dataService.Get(existingKey);

        // assert
        Assert.That(result.Content, Is.EqualTo(existingContent));
    }

    [Test]
    public async Task Get_returns_error_message_for_a_key_that_hasnt_been_added_yet()
    {
        // arrange
        var bogusKey = new Bogus.DataSets.Hacker().Noun() + _random.Next(10000);
        var expectedContent = $"ERROR: Content Not Found ({bogusKey})";

        // act
        var result = await _dataService.Get(bogusKey);

        // assert
        Assert.That(result.Content, Is.EqualTo(expectedContent));
    }
}