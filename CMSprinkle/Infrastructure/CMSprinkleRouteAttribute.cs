﻿using Microsoft.AspNetCore.Mvc;

namespace CMSprinkle.Infrastructure;

internal class CMSprinkleRouteAttribute : RouteAttribute{

    public const string DEFAULT_ROUTE_PREFIX = "cmsprinkle";
    public static string RoutePrefix = DEFAULT_ROUTE_PREFIX;

    public CMSprinkleRouteAttribute(string routeTemplate)
        : base(GetPrefixedRoute(routeTemplate))
    {
    }

    private static string GetPrefixedRoute(string template)
    {
        // normalize the prefix and template
        string prefix = RoutePrefix.TrimEnd('/');
        string formattedTemplate = template.Trim('/');

        return $"/{prefix}/{formattedTemplate}";
    }
}