using System;

namespace CMSprinkle.Infrastructure;

public class CMSprinkleOptions
{
    /// <summary>
    /// Route prefix for the CMSprinkle URLs
    /// Default is "cmsprinkle"
    /// Example: /cmsprinkle/home
    /// </summary>
    public string RoutePrefix { get; set; }

    /// <summary>
    /// Define this Func to return a custom message that will appear
    /// when you haven't created the content key yet.
    /// The parameter is the content key.
    /// </summary>
    public Func<string, string> ContentNotFoundMessage { get; set; }
}