using CMSprinkle.Data;
using CMSprinkle.SqlServer.Tests.TestHelpers;
using CMSprinkle.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.SqlServer.Tests.SqlServerCMSprinkleDataServiceTests;

[TestFixture]
public class DeleteTests : SqlServerIntegrationTest
{
    private ICMSprinkleDataService _dataService;

    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();

        var tableName = $"table_{_random.Next(100000)}";
        _serviceCollection.AddCMSprinkleSqlServer(_connectionString, tableName, _schemaName, true);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        await _dataService.InitializeDatabase();
    }

    [Test]
    public async Task Delete_removes_content_from_database()
    {
        // arrange
        var key = $"contentkey-{_random.Next(10000)}";
        await _dataService.AddNew(new AddContentSubmitModel
        {
            Key = key,
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        });

        // act
        await _dataService.Delete(key);

        // assert
        var getBack = await _dataService.Get(key);
        Assert.That(getBack.Content, Is.EqualTo(GetContentResult.DefaultNotFoundMessage(key)));
    }

    [Test]
    public async Task Delete_removes_content_from_index()
    {
        // arrange
        var key = $"contentkey-{_random.Next(10000)}";
        await _dataService.AddNew(new AddContentSubmitModel
        {
            Key = key,
            Content = new Bogus.DataSets.Lorem().Paragraphs(3)
        });

        // act
        await _dataService.Delete(key);

        // assert
        var getBack = await _dataService.GetAllForHome();
        Assert.That(getBack.AllContent.Any(c => c.ContentKey == key), Is.False);
    }


    [Test]
    public async Task Delete_operation_on_nonexistent_throws_exception()
    {
        // arrange
        var key = $"contentkey-{_random.Next(10000)}";

        // act
        var ex = Assert.CatchAsync(async () => await _dataService.Delete(key));

        // assert
        Assert.That(ex.AllExceptions().Any(x => x.Message.Contains("No rows deleted")), Is.True);
    }

}