using System.Data;
using CMSprinkle.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.SqlServer;

public static class SqlServerServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQL Server as the database backend for CMSprinkle. The database/schema must already exist.
    /// </summary>
    /// <param name="connectionString">Relational database connection string.</param>
    /// <param name="tableName">Relational table name</param>
    /// <param name="schemaName">(optional) Relational schema name (dbo by default)</param>
    /// <param name="createTableIfNecessary">(optional) Creates the table for tableName if necessary (True by default)</param>
    public static IServiceCollection AddCMSprinkleSqlServer(this IServiceCollection @this,
        string connectionString,
        string tableName,
        string schemaName = "dbo",
        bool createTableIfNecessary = true)
    {
        @this.AddSingleton<IDbConnection>(x => new SqlConnection(connectionString));

        // add SQL Server data service for CMSPrinkle
        @this.AddTransient<ICMSprinkleDataService, SqlServerCMSprinkleDataService>();

        // wrapper so that table name can be injected
        @this.AddSingleton<SqlServerSettings>(x => new SqlServerSettings
        {
            SchemaName = schemaName,
            TableName = tableName,
            CreateTableIfNecessary = createTableIfNecessary
        });

        return @this;
    }
}