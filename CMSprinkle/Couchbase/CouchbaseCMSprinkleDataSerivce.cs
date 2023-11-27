using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMSprinkle.Data;
using CMSprinkle.ViewModels;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Couchbase.Transactions;
using Couchbase.Transactions.Config;
using Ganss.Xss;
using Markdig;

namespace CMSprinkle.Couchbase;

public class CouchbaseCMSprinkleDataSerivce : ICMSprinkleDataService
{
    private readonly ICmsCollectionProvider _cmsCollectionProvider;
    private readonly DurabilityLevelWrapper _durabilityLevelWrapper;
    private readonly ICMSprinkleAuth _auth;

    public CouchbaseCMSprinkleDataSerivce(ICmsCollectionProvider cmsCollectionProvider, DurabilityLevelWrapper durabilityLevelWrapper, ICMSprinkleAuth auth)
    {
        _cmsCollectionProvider = cmsCollectionProvider;
        _durabilityLevelWrapper = durabilityLevelWrapper;
        _auth = auth;
    }

    public async Task<GetContentResult> Get(string contentKey)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();

        var contentDoc = await collection.TryGetAsync(MakeCouchbaseKey(contentKey));
        if (!contentDoc.Exists)
            return new GetContentResult { Key = contentKey, Content = null, LastUser = null};
        var content = contentDoc.ContentAs<CMSprinkleContent>();

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        var html = Markdown.ToHtml(content.Content, pipeline);

        var sanitizer = new HtmlSanitizer();
        return new GetContentResult
        {
            Key = contentKey,
            Content = sanitizer.Sanitize(html),
            LastUser = content.LastUser
        };
    }

    public async Task<string> GetAdmin(string contentKey)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var contentDoc = await collection.GetAsync(MakeCouchbaseKey(contentKey));
        var content = contentDoc.ContentAs<CMSprinkleContent>();
        return content.Content;
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
            LastUser = _auth.GetUsername(),
            CreatedAt = DateTimeOffset.Now,
            UpdatedLast = DateTimeOffset.Now
        };

        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var cluster = collection.Scope.Bucket.Cluster;

        var transaction = Transactions.Create(cluster, 
            TransactionConfigBuilder.Create().DurabilityLevel(_durabilityLevelWrapper.DurabilityLevel));
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

        await collection.MutateInAsync(MakeCouchbaseKey(contentKey), specs =>
        {
            specs.Upsert("content", model.Content);
            specs.Upsert("lastUser", _auth.GetUsername());
            specs.Upsert("updatedLast", DateTimeOffset.Now);
            // do not ever replace createdAt
        });
    }

    public async Task Delete(string contentKey)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var cluster = collection.Scope.Bucket.Cluster;

        var transaction = Transactions.Create(cluster,
            TransactionConfigBuilder.Create().DurabilityLevel(_durabilityLevelWrapper.DurabilityLevel));
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