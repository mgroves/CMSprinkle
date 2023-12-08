using System.Data;
using Dapper;

namespace CMSprinkle.SqlServer.Tests.TestHelpers;

public static class DbConnectionExtensions
{
    public static async Task<bool> TableExists(this IDbConnection @this, string schemaName, string tableName)
    {
        var result = await @this.ExecuteScalarAsync<int>(@"
            IF EXISTS (
                SELECT 1 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = @schemaName 
                AND TABLE_NAME = @tableName
            )
            SELECT 1
            ELSE
            SELECT 0", new { schemaName, tableName });
        return result == 1;
    }
}