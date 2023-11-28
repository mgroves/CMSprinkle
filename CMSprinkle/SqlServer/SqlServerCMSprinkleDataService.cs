using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CMSprinkle.Data;
using CMSprinkle.ViewModels;
using Dapper;
using Ganss.Xss;
using Markdig;

namespace CMSprinkle.SqlServer;

public class SqlServerCMSprinkleDataService : ICMSprinkleDataService
{
    private readonly IDbConnection _dbConnection;
    private readonly ICMSprinkleAuth _auth;
    private readonly string _tableName;
    private readonly string _schemaName;

    public SqlServerCMSprinkleDataService(IDbConnection dbConnection, ICMSprinkleAuth auth, TableNameWrapper tableNameWrapper)
    {
        _dbConnection = dbConnection;
        _auth = auth;
        _tableName = tableNameWrapper.TableName;
        _schemaName = tableNameWrapper.SchemaName;
    }

    public async Task<string> GetAdmin(string contentKey)
    {
        var result = await _dbConnection.QueryFirstAsync<CMSprinkleContent>($@"
            SELECT Content FROM [{_schemaName}].[{_tableName}]");
        return result.Content;
    }

    public async Task<GetContentResult> Get(string contentKey)
    {
        var result = await _dbConnection.QueryFirstOrDefaultAsync<CMSprinkleContent>($@"
            SELECT Content FROM [{_schemaName}].[{_tableName}]");

        if (result == null)
            return new GetContentResult { Key = contentKey, Content = null, LastUser = null };

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        var html = Markdown.ToHtml(result.Content, pipeline);

        var sanitizer = new HtmlSanitizer();
        return new GetContentResult
        {
            Key = contentKey,
            Content = sanitizer.Sanitize(html),
            LastUser = result.LastUser
        };
    }

    public async Task<CMSprinkleHome> GetAllForHome()
    {
        var result = await _dbConnection.QueryAsync<CMSprinkleContent>($@"
            SELECT * FROM [{_schemaName}].[{_tableName}]");
        return new CMSprinkleHome { AllContent = result.ToList()};
    }

    public async Task AddNew(AddContentSubmitModel model)
    {
        await _dbConnection.ExecuteAsync($@"
            INSERT INTO [{_schemaName}].[{_tableName}] (ContentKey, Content, LastUser, CreatedAt, UpdatedLast)
            VALUES (@ContentKey, @Content, @LastUser, @CreatedAt, @UpdatedLast)", new
        {
            ContentKey = model.Key,
            Content = model.Content,
            LastUser = _auth.GetUsername(),
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
            LastUser = _auth.GetUsername(),
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