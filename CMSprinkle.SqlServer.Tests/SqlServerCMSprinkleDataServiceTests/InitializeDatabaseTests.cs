using System.Data;
using CMSprinkle.Data;
using CMSprinkle.SqlServer.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.SqlServer.Tests.SqlServerCMSprinkleDataServiceTests;

[TestFixture]
public class InitializeDatabaseTests : SqlServerIntegrationTest
{
    [Test]
    public async Task Table_gets_created_by_default()
    {
        // arrange
        var tableName = $"table_{_random.Next(100000)}";
        _serviceCollection.AddCMSprinkleSqlServer(_connectionString, tableName, _schemaName, true);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        var db = serviceProvider.GetService<IDbConnection>();

        // act
        await dataService.InitializeDatabase();

        // assert
        Assert.That(await db.TableExists(_schemaName, tableName), Is.True);
    }

    [Test]
    public async Task Table_set_to_not_created()
    {
        // arrange
        var tableName = $"table_{_random.Next(100000)}";
        _serviceCollection.AddCMSprinkleSqlServer(_connectionString, tableName, _schemaName, false);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var dataService = serviceProvider.GetService<ICMSprinkleDataService>();
        var db = serviceProvider.GetService<IDbConnection>();

        // act
        await dataService.InitializeDatabase();

        // assert
        Assert.That(await db.TableExists(_schemaName, tableName), Is.False);
    }
}