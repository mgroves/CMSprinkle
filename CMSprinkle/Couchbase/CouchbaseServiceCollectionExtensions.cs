using CMSprinkle.Data;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.Couchbase;

public static class CouchbaseServiceCollectionExtensions
{
    /// <summary>
    /// Add Couchbase as the database backend for CMSprinkle. The bucket/scope/collection must already exist.
    /// </summary>
    /// <param name="bucketName">Couchbase bucket name</param>
    /// <param name="scopeName">Couchbase scope name</param>
    /// <param name="collectionName">Couchbase collection name</param>
    /// <param name="durabilityLevel">(optional) ACID durability level (None by default)</param>
    public static void AddCMSprinkleCouchbase(this IServiceCollection @this,
        string bucketName,
        string scopeName,
        string collectionName,
        DurabilityLevel durabilityLevel = DurabilityLevel.None)
    {
        @this.AddCouchbaseBucket<ICmsBucketProvider>(bucketName, b =>
        {
            b
                .AddScope(scopeName)
                .AddCollection<ICmsCollectionProvider>(collectionName);
        });
        @this.AddTransient<ICMSprinkleDataService, CouchbaseCMSprinkleDataSerivce>();
        @this.AddSingleton<DurabilityLevelWrapper>(x => new DurabilityLevelWrapper(durabilityLevel));
    }
}