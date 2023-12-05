using System.Data;
using CMSprinkle.Auth;
using CMSprinkle.Data;
using CMSprinkle.Infrastructure;
using CMSprinkle.ViewModels;
using Dapper;

namespace CMSprinkle.SqlServer;

public class SqlServerCMSprinkleDataService : ICMSprinkleDataService
{
    private readonly IDbConnection _dbConnection;
    private readonly ICMSprinkleAuth _auth;
    private readonly SqlServerSettings _settings;
    private readonly string _tableName;
    private readonly string _schemaName;

    public SqlServerCMSprinkleDataService(IDbConnection dbConnection, ICMSprinkleAuth auth, SqlServerSettings settings)
    {
        _dbConnection = dbConnection;
        _auth = auth;
        _settings = settings;
        _tableName = _settings.TableName;
        _schemaName = _settings.SchemaName;
    }

    public async Task InitializeDatabase()
    {
        if (!_settings.CreateTableIfNecessary)
            return;

        string sql = @$"
        DECLARE @DynamicTableName NVARCHAR(255);
        SET @DynamicTableName = QUOTENAME(@SchemaName) + '.' + QUOTENAME(@TableName);

        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @SchemaName AND TABLE_NAME = @TableName)
        BEGIN
            EXEC('CREATE TABLE ' + @DynamicTableName + ' (
                ContentKey NVARCHAR(90) PRIMARY KEY,
                Content NVARCHAR(MAX),
                LastUser NVARCHAR(255),
                CreatedAt DATETIME,
                UpdatedLast DATETIME NULL
            )');
        END";
        await _dbConnection.ExecuteAsync(sql, new { TableName = _tableName, SchemaName = _schemaName });
    }

    public async Task<GetContentResult> Get(string contentKey)
    {
        var result = await _dbConnection.QueryFirstOrDefaultAsync<CMSprinkleContent>($@"
            SELECT Content FROM [{_schemaName}].[{_tableName}] WHERE ContentKey = @contentKey", new { contentKey });

        if (result == null)
            return new GetContentResult { Key = contentKey, Content = null };

        return new GetContentResult
        {
            Key = contentKey,
            Content = result.Content
        };
    }

    public async Task<CMSprinkleHome> GetAllForHome()
    {
        var result = await _dbConnection.QueryAsync<CMSprinkleContent>($@"
            SELECT * FROM [{_schemaName}].[{_tableName}]");
        return new CMSprinkleHome { AllContent = result.ToList() };
    }

    public async Task AddNew(AddContentSubmitModel model)
    {
        await _dbConnection.ExecuteAsync($@"
            INSERT INTO [{_schemaName}].[{_tableName}] (ContentKey, Content, LastUser, CreatedAt, UpdatedLast)
            VALUES (@ContentKey, @Content, @LastUser, @CreatedAt, @UpdatedLast)", new
        {
            ContentKey = model.Key,
            Content = model.Content,
            LastUser = await _auth.GetUsername(),
            CreatedAt = DateTimeOffset.Now,
            UpdatedLast = DateTimeOffset.Now
        });
    }

    public async Task Update(string contentKey, EditContentSubmitModel model)
    {
        await _dbConnection.ExecuteAsync($@"
            UPDATE [{_schemaName}].[{_tableName}]
            SET Content = @Content,
                LastUser = @LastUser,
                UpdatedLast = @UpdatedLast
            WHERE ContentKey = @ContentKey
        ", new
        {
            ContentKey = contentKey,
            Content = model.Content,
            LastUser = await _auth.GetUsername(),
            UpdatedLast = DateTimeOffset.Now
            // do not update CreatedAt
        });
    }

    public async Task Delete(string contentKey)
    {
        await _dbConnection.ExecuteAsync(@$"
            DELETE FROM [{_schemaName}].[{_tableName}]
            WHERE ContentKey = @ContentKey", new
        {
            ContentKey = contentKey
        });
    }
}