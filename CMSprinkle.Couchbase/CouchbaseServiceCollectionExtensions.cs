﻿using System.Threading.Tasks;
using CMSprinkle.Data;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Couchbase.Management.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace CMSprinkle.Couchbase;

public static class CouchbaseServiceCollectionExtensions
{
    /// <summary>
    /// Add Couchbase as the database backend for CMSprinkle. The bucket/scope must already exist.
    /// </summary>
    /// <param name="bucketName">Couchbase bucket name</param>
    /// <param name="scopeName">Couchbase scope name</param>
    /// <param name="collectionName">Couchbase collection name</param>
    /// <param name="durabilityLevel">(optional) ACID durability level (None by default)</param>
    /// <param name="createCollectionIfNecessary">(optional) Creates the collection for collectionName if necessary (True by default)</param>
    public static IServiceCollection AddCMSprinkleCouchbase(this IServiceCollection @this,
        string bucketName,
        string scopeName,
        string collectionName,
        DurabilityLevel durabilityLevel = DurabilityLevel.None,
        bool createCollectionIfNecessary = true)
    {
        // add couchbase bucket/collection providers, used by the data service
        @this.AddCouchbaseBucket<ICmsBucketProvider>(bucketName, b =>
        {
            b
                .AddScope(scopeName)
                .AddCollection<ICmsCollectionProvider>(collectionName);
        });

        // add couchbase data service for CMSPrinkle
        @this.AddTransient<ICMSprinkleDataService, CouchbaseCMSprinkleDataService>();

        // wrapper so that durability level enum can be injected
        @this.AddSingleton<CouchbaseSettings>(x => new CouchbaseSettings
        {
            CollectionName = collectionName,
            ScopeName = scopeName,
            DurabilityLevel = durabilityLevel,
            CreateCollectionIfNecessary = createCollectionIfNecessary
        });

        return @this;
    }
}