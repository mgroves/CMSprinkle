using CMSprinkle.Data;
using CMSprinkle.Couchbase.Tests.TestHelpers;
using CMSprinkle.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.Couchbase.Tests.CouchbaseCMSprinkleDataServiceTests;

public class AddNewTests : CouchbaseIntegrationTest
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
    public async Task After_adding_a_new_item_it_shows_up_in_home_view()
    {
        // arrange
        var model = new AddContentSubmitModel();
        model.Key = $"key-{_random.Next(100000)}";
        model.Content = new Bogus.DataSets.Lorem().Paragraphs(3);

        // act
        await _dataService.AddNew(model);
        var all = await _dataService.GetAllForHome();

        // assert
        Assert.That(all.AllContent.Any(c => c.ContentKey == model.Key), Is.True);
        Assert.That(all.AllContent.Any(c => c.Content == model.Content), Is.True);
    }

    [Test]
    public async Task Trying_to_add_duplicate_key_throws_exception()
    {
        // arrange
        var model = new AddContentSubmitModel();
        model.Key = $"key-{_random.Next(100000)}";
        model.Content = new Bogus.DataSets.Lorem().Paragraphs(3);
        await _dataService.AddNew(model);

        // act
        var ex = Assert.CatchAsync(async () =>
        {
            await _dataService.AddNew(model);
        });

        // assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.AllExceptions().Any(x => x.Message == $"Content key '{model.Key}' already exists."), Is.True);
    }

    [Test]
    public async Task Adding_content_stamps_with_time_and_username()
    {
        // arrange
        var model = new AddContentSubmitModel();
        model.Key = $"key-{_random.Next(100000)}";
        model.Content = new Bogus.DataSets.Lorem().Paragraphs(3);
        var rightNow = DateTimeOffset.Now;

        // act
        await _dataService.AddNew(model);

        // assert
        var allContent = await _dataService.GetAllForHome();
        var thisContent = allContent.AllContent.FirstOrDefault(c => c.ContentKey == model.Key);
        Assert.That(thisContent.LastUser, Is.Not.Null.Or.Empty);
        Assert.That(thisContent.UpdatedLast, Is.EqualTo(rightNow).Within(2).Seconds);
    }
}