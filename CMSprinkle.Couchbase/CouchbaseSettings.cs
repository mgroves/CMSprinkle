using Couchbase.KeyValue;

namespace CMSprinkle.Couchbase;

public class CouchbaseSettings
{
    public required DurabilityLevel DurabilityLevel { get; set; }
    public required string CollectionName { get; set; }
    public required string ScopeName { get; set; }
    public required bool CreateCollectionIfNecessary { get; set; }
}