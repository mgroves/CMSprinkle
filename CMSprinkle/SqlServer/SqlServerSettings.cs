namespace CMSprinkle.SqlServer;

public class SqlServerSettings
{
    public required string TableName { get; set; }
    public required string SchemaName { get; set; }
    public required bool CreateTableIfNecessary { get; set; }
}