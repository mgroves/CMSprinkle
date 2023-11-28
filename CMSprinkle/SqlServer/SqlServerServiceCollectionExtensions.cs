using System.Data;
using System.Threading.Tasks;
using CMSprinkle.Data;
using Dapper;
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
    public static async Task AddCMSprinkleSqlServerAsync(this IServiceCollection @this,
        string connectionString,
        string tableName,
        string schemaName = "dbo",
        bool createTableIfNecessary = true)
    {
        @this.AddSingleton<IDbConnection>(x => new SqlConnection(connectionString));

        // create table if necessary
        var tempServiceLocator = @this.BuildServiceProvider();
        var dbConnection = tempServiceLocator.GetService<IDbConnection>();
        string sql = @$"
            DECLARE @DynamicTableName NVARCHAR(255);
            SET @DynamicTableName = QUOTENAME(@TableName);

            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName)
            BEGIN
                EXEC('CREATE TABLE ' + @DynamicTableName + ' (
                    ContentKey NVARCHAR(90) PRIMARY KEY,
                    Content NVARCHAR(MAX),
                    LastUser NVARCHAR(255),
                    CreatedAt DATETIME,
                    UpdatedLast DATETIME NULL
                )');
            END";
        await dbConnection.ExecuteAsync(sql, new { TableName = tableName });

        // add SQL Server data service for CMSPrinkle
        @this.AddTransient<ICMSprinkleDataService, SqlServerCMSprinkleDataService>();

        // wrapper so that table name can be injected
        @this.AddSingleton<TableNameWrapper>(x => new TableNameWrapper(tableName, schemaName));
    }
}