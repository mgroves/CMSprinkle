namespace CMSprinkle.Couchbase.Tests.TestHelpers;

public static class BucketProviderExtensions
{
    public static async Task<bool> CollectionExists(this ICmsBucketProvider @this, string scopeName,
        string collectionName)
    {
        var bucket = await @this.GetBucketAsync();
        var collManager = bucket.Collections;
        var allScopes = await collManager.GetAllScopesAsync();
        var collection = allScopes.FirstOrDefault(s => s.Collections.Any(c => c.Name == collectionName));
        return collection != null;
    }

    public static async Task<bool> DocumentExists(this ICmsBucketProvider @this, string scopeName,
        string collectionName, string documentKey)
    {
        var bucket = await @this.GetBucketAsync();
        var scope = await bucket.ScopeAsync(scopeName);
        var collection = await scope.CollectionAsync(collectionName);
        var exists = await collection.ExistsAsync(documentKey);
        return exists.Exists;
    }
}