using System;
using CMSprinkle.Data;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;

namespace CMSprinkle.Couchbase;

public static class ServiceCollectionExtensions
{
    public static void AddCMSprinkle(this IServiceCollection @this, Action<CMSprinkleOptions>? options = null)
    {
        @this.AddControllersWithViews()
            .AddApplicationPart(typeof(CMSprinkleContent).Assembly)
            .AddRazorRuntimeCompilation();

        @this.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            { options.FileProviders.Add(new EmbeddedFileProvider(typeof(CMSprinkleContent).Assembly)); });

        // doesn't really do anything, but might want options later
        var opts = new CMSprinkleOptions();
        if(options != null)
            options(opts);

        var serviceProvider = @this.BuildServiceProvider();
        var authService = serviceProvider.GetService<ICMSprinkleAuth>();
        if (authService == null)
        {
            @this.AddTransient<ICMSprinkleAuth, DefaultLocalOnlyAuth>();
        }
    }


    public static void AddCMSprinkleCouchbase(this IServiceCollection @this,
        string bucketName,
        string scopeName,
        string collectionName)
    {
        @this.AddCouchbaseBucket<ICmsBucketProvider>(bucketName, b =>
        {
            b
                .AddScope(scopeName)
                .AddCollection<ICmsCollectionProvider>(collectionName);
        });
        @this.AddTransient<ICMSprinkleDataService, CouchbaseCMSprinkleDataSerivce>();
    }
}