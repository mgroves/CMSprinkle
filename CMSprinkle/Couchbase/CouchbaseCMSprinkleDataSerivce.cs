using System.Collections.Generic;
using System.Threading.Tasks;
using CMSprinkle.Data;
using CMSprinkle.ViewModels;
using Couchbase.KeyValue;
using Ganss.Xss;
using Markdig;

namespace CMSprinkle.Couchbase;

public class CouchbaseCMSprinkleDataSerivce : ICMSprinkleDataService
{
    private readonly ICmsCollectionProvider _cmsCollectionProvider;

    public CouchbaseCMSprinkleDataSerivce(ICmsCollectionProvider cmsCollectionProvider)
    {
        _cmsCollectionProvider = cmsCollectionProvider;
    }

    public async Task<GetContentResult> Get(string contentKey)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();

        var contentDoc = await collection.TryGetAsync("content::" + contentKey);
        if (!contentDoc.Exists)
            return new GetContentResult { Key = contentKey, Content = null};
        var content = contentDoc.ContentAs<CMSprinkleContent>();

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        var html = Markdown.ToHtml(content.Content, pipeline);

        var sanitizer = new HtmlSanitizer();
        return new GetContentResult
        {
            Key = contentKey,
            Content = sanitizer.Sanitize(html)
        };
    }

    public async Task<string> GetAdmin(string contentKey)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var contentDoc = await collection.GetAsync("content::" + contentKey);
        var content = contentDoc.ContentAs<CMSprinkleContent>();
        return content.Content;
    }

    public async Task<CMSprinkleHome> GetAllForHome()
    {
        var homeView = new CMSprinkleHome();
        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var index = collection.Set<string>("ContentIndex");
        var allContent = new Dictionary<string, string>();
        foreach (var i in index)
        {
            var contentResult = await collection.GetAsync("content::" + i);
            var content = contentResult.ContentAs<CMSprinkleContent>();
            allContent.Add(i, content.Content);
        }
        homeView.AllContent = allContent;
        return homeView;
    }

    public async Task AddNew(AddContentSubmitModel model)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();
        var index = collection.Set<string>("ContentIndex");

        // TODO: ACID
        await index.AddAsync(model.Key);
        await collection.InsertAsync("content::" + model.Key, new CMSprinkleContent() { Content = model.Content} );
    }

    public async Task Update(string contentKey, EditContentSubmitModel model)
    {
        var collection = await _cmsCollectionProvider.GetCollectionAsync();

        await collection.ReplaceAsync("content::" + contentKey, new CMSprinkleContent() { Content = model.Content });
    }
}