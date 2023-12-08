using CMSprinkle.Data;
using CMSprinkle.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.SqlServer.Tests.SqlServerCMSprinkleDataServiceTests;

[TestFixture]
public class GetAllForHomeTests : SqlServerIntegrationTest
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
    public async Task All_content_items_are_given_to_the_home_page()
    {
        // arrange
        var numContent = _random.Next(1, 13);
        for (var i = 0; i < numContent; i++)
        {
            await _dataService.AddNew(new AddContentSubmitModel
            {
                Key = $"key-{_random.Next(100000)}",
                Content = new Bogus.DataSets.Lorem().Paragraphs(3)
            });
        }

        // act
        var result = await _dataService.GetAllForHome();

        // asset
        Assert.That(result.AllContent.Count, Is.EqualTo(numContent));
    }
}