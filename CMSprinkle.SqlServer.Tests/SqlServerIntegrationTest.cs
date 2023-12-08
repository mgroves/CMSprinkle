using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CMSprinkle.Infrastructure;
using Nito.AsyncEx;
using Testcontainers.MsSql;

namespace CMSprinkle.SqlServer.Tests;

public abstract class SqlServerIntegrationTest
{
    protected static AsyncLazy<MsSqlContainer> _sqlServerContainer = new(async () =>
    {
        var sqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("yourStrong(!)Password")
            .Build();

        await sqlServerContainer.StartAsync();

        return sqlServerContainer;
    });

    protected ServiceCollection? _serviceCollection;
    protected Random _random;
    protected string _schemaName;
    protected string _connectionString;

    [SetUp]
    public virtual async Task Setup()
    {
        var sqlContainer = await _sqlServerContainer;
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddCMSprinkle();
        _serviceCollection.AddHttpContextAccessor();
        _serviceCollection.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Debug);
        });
        _connectionString = sqlContainer.GetConnectionString();
        _random = new Random();
        _schemaName = "dbo";
    }
}