﻿using System;
using CMSprinkle.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using CMSprinkle.Auth;

namespace CMSprinkle.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddCMSprinkle(this IServiceCollection @this, Action<CMSprinkleOptions> options = null)
    {
        // add the controllers/views from CMSprinkle
        @this.AddControllersWithViews()
            .AddApplicationPart(typeof(CMSprinkleContent).Assembly)
            .AddRazorRuntimeCompilation();
        @this.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            { options.FileProviders.Add(new EmbeddedFileProvider(typeof(CMSprinkleContent).Assembly)); });

        // get options, if any were specified
        var opts = new CMSprinkleOptions();
        if (options != null)
            options(opts);

        // route prefix
        if (!string.IsNullOrEmpty(opts.RoutePrefix))
        {
            CMSprinkleRouteAttribute.RoutePrefix = opts.RoutePrefix;
        }

        // missing content error message
        if (opts.ContentNotFoundMessage != null)
            GetContentResult.ContentNotFoundMessage = opts.ContentNotFoundMessage;
        else
            GetContentResult.ContentNotFoundMessage = x => $"ERROR: Content Not Found ({x})";

        // if there is no ICMSprinkleAuth implementation registered,
        // then fall back to local only access
        var serviceProvider = @this.BuildServiceProvider();
        var authService = serviceProvider.GetService<ICMSprinkleAuth>();
        if (authService == null)
        {
            @this.AddTransient<ICMSprinkleAuth, DefaultLocalOnlyAuth>();
        }
    }
}