using Couchbase.KeyValue;

namespace CMSprinkle.SqlServer;

public class TableNameWrapper
{
    public string TableName { get; }
    public string SchemaName { get; }

    public TableNameWrapper(string tableName, string schemaName)
    {
        TableName = tableName;
        SchemaName = schemaName;
    }
}