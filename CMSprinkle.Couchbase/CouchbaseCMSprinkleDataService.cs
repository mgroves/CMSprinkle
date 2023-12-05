using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMSprinkle.Auth;
using CMSprinkle.Data;
using CMSprinkle.Infrastructure;
using CMSprinkle.ViewModels;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Couchbase.Management.Collections;
using Couchbase.Transactions;
using Couchbase.Transactions.Config;

namespace CMSprinkle.Couchbase;

public class CouchbaseCMSprinkleDataService : ICMSprinkleDataService
{
    private readonly ICmsCollectionProvider _cmsCollectionProvider;
    private readonly CouchbaseSettings _settings;
    private readonly ICMSprinkleAuth _auth;
    private readonly ICmsBucketProvider _bucketProvider;

    public CouchbaseCMSprinkleDataService(ICmsCollectionProvider cmsCollectionProvider, CouchbaseSettings settings, ICMSprinkleAuth auth, ICmsBucketProvider bucketProvider)
    {
        _cmsCollectionProvider = cmsCollectionProvider;
        _settings = settings;
        _auth = auth;
        _bucketProvider = bucketProvider;
    }

    public async Task InitializeDatabase()
    {
        if (!_settings.CreateCollectionIfNecessary)
            return;

        var bucket = await _bucketProvider.GetBucketAsync();
        if (_settings.CollectionName != "_default")
        {
            var collectionManager = bucket.Collections;
            try
            {
                await collectionManager.CreateCollectionAsync(_settings.ScopeName, _settings.CollectionName,
                    new CreateCollectionSettings());
            }
            catch (CollectionExistsException)
            {
                // I hate using a try/catch for this
                // is there a better way?
            }
        }

        // create index document if necessary
        // i'm using this as a way to avoid SQL++
        // trying to keep CMSprinkle lightweight, using pure k/v
        var scope = await bucket.ScopeAsync(_settings.ScopeName);
        var collection = await scope.CollectionAsync(_settings.CollectionName);
        var indexDocExists = await collection.ExistsAsync("ContentIndex");
        if (indexDocExists.Exists)
            return;
        await collection.InsertAsync("ContentIndex", new List<string>());
    }

    public async Task<GetContentResult> Get(string contentKey)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();

        var contentDoc = await collection.TryGetAsync(MakeCouchbaseKey(contentKey));
        if (!contentDoc.Exists)
            return new GetContentResult { Key = contentKey, Content = null};

        var content = contentDoc.ContentAs<CMSprinkleContent>();

        return new GetContentResult
        {
            Key = contentKey,
            Content = content.Content
        };
    }

    public async Task<CMSprinkleHome> GetAllForHome()
    {
        var homeView = new CMSprinkleHome();
        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var index = collection.Set<string>("ContentIndex");
        homeView.AllContent = new List<CMSprinkleContent>();
        foreach (var i in index)
        {
            var contentResult = await collection.GetAsync(MakeCouchbaseKey(i));
            var content = contentResult.ContentAs<CMSprinkleContent>();
            content.ContentKey = i;
            homeView.AllContent.Add(content);
        }
        return homeView;
    }

    public async Task AddNew(AddContentSubmitModel model)
    {
        var contentToSave = new CMSprinkleContent
        {
            ContentKey = model.Key,
            Content = model.Content,
            LastUser = await _auth.GetUsername(),
            CreatedAt = DateTimeOffset.Now,
            UpdatedLast = DateTimeOffset.Now
        };

        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var cluster = collection.Scope.Bucket.Cluster;

        var transaction = Transactions.Create(cluster, 
            TransactionConfigBuilder.Create().DurabilityLevel(_settings.DurabilityLevel));
        await transaction.RunAsync(async ctx =>
        {
            // add key to index, unless it's already there
            var indexResult = await ctx.GetAsync(collection, "ContentIndex");
            var index = indexResult.ContentAs<List<string>>();
            if (index.Contains(model.Key))
            {
                // key already exists!
                throw new DocumentExistsException($"Content key '{model.Key}' already exists.");
            }
            else
            {
                index.Add(model.Key);
            }
            // update index, add new content doc
            await ctx.ReplaceAsync(indexResult, index);
            await ctx.InsertAsync(collection, MakeCouchbaseKey(model.Key), contentToSave);
        });
    }

    public async Task Update(string contentKey, EditContentSubmitModel model)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();

        var userName = await _auth.GetUsername();
        await collection.MutateInAsync(MakeCouchbaseKey(contentKey), specs =>
        {
            specs.Upsert("content", model.Content);
            specs.Upsert("lastUser", userName);
            specs.Upsert("updatedLast", DateTimeOffset.Now);
            // do not ever replace createdAt
        });
    }

    public async Task Delete(string contentKey)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var cluster = collection.Scope.Bucket.Cluster;

        var transaction = Transactions.Create(cluster,
            TransactionConfigBuilder.Create().DurabilityLevel(_settings.DurabilityLevel));
        await transaction.RunAsync(async ctx =>
        {
            // get index
            var indexResult = await ctx.GetAsync(collection, "ContentIndex");
            var index = indexResult.ContentAs<List<string>>();
            index.Remove(contentKey);

            // get the doc
            var contentToDelete = await ctx.GetAsync(collection, MakeCouchbaseKey(contentKey));

            // remove from index, remove content doc
            await ctx.ReplaceAsync(indexResult, index);
            await ctx.RemoveAsync(contentToDelete);
        });
    }

    private string MakeCouchbaseKey(string contentKey)
    {
        return $"content::{contentKey}";
    }
}