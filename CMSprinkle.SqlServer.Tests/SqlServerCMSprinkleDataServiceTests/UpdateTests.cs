using CMSprinkle.Auth;
using CMSprinkle.Data;
using CMSprinkle.SqlServer.Tests.TestHelpers;
using CMSprinkle.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.SqlServer.Tests.SqlServerCMSprinkleDataServiceTests;

[TestFixture]
public class UpdateTests : SqlServerIntegrationTest
{
    private ICMSprinkleDataService _dataService;
    private FakeAuth _fakeAuth;

    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();

        _fakeAuth = new FakeAuth();
        _fakeAuth.FakeGetUsername = "first-user-{_random.Next(100000)}";

        var tableName = $"table_{_random.Next(100000)}";
        _serviceCollection.AddTransient<ICMSprinkleAuth>(_ => _fakeAuth);
        _serviceCollection.AddCMSprinkleSqlServer(_connectionString, tableName, _schemaName, true);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        await _dataService.InitializeDatabase();
    }

    [Test]
    public async Task Updating_content()
    {
        // arrange
        var contentKey = $"key_{_random.Next(100000)}";
        var addModel = new AddContentSubmitModel
        {
            Key = contentKey,
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        };
        await _dataService.AddNew(addModel);
        var updateModel = new EditContentSubmitModel
        {
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        };
        
        // act
        await _dataService.Update(contentKey, updateModel);
        var contentBackOut = await _dataService.Get(contentKey);

        // assert
        Assert.That(contentBackOut.Content, Is.EqualTo(updateModel.Content));
    }

    [Test]
    public async Task Updating_content_changes_username()
    {
        // arrange
        var firstUsername = $"first-user-{_random.Next(100000)}";
        _fakeAuth.FakeGetUsername = firstUsername;
        var contentKey = $"key_{_random.Next(100000)}";
        var addModel = new AddContentSubmitModel
        {
            Key = contentKey,
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        };
        await _dataService.AddNew(addModel);

        var secondUsername = $"second-user-{_random.Next(100000)}";
        _fakeAuth.FakeGetUsername = secondUsername;
        var updateModel = new EditContentSubmitModel
        {
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        };

        // act
        await _dataService.Update(contentKey, updateModel);
        var all = await _dataService.GetAllForHome();
        var updated = all.AllContent.First(c => c.ContentKey == contentKey);

        // assert
        Assert.That(updated.LastUser, Is.EqualTo(secondUsername));
    }

    [Test]
    public async Task Updating_content_changes_timestamp_of_updatedlast_but_not_createdat()
    {
        // arrange
        var contentKey = $"key_{_random.Next(100000)}";
        var addModel = new AddContentSubmitModel
        {
            Key = contentKey,
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        };
        await _dataService.AddNew(addModel);
        var all1 = await _dataService.GetAllForHome();
        var original = all1.AllContent.First(c => c.ContentKey == contentKey);

        var updateModel = new EditContentSubmitModel
        {
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        };

        // act
        await _dataService.Update(contentKey, updateModel);
        var all2 = await _dataService.GetAllForHome();
        var updated = all2.AllContent.First(c => c.ContentKey == contentKey);

        // assert
        Assert.That(updated.UpdatedLast, Is.Not.EqualTo(original.CreatedAt));
        Assert.That(updated.UpdatedLast, Is.Not.EqualTo(original.UpdatedLast));
        Assert.That(updated.CreatedAt, Is.EqualTo(original.CreatedAt));
    }
}