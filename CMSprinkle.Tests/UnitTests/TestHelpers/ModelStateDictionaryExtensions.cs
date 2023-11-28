using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CMSprinkle.Tests.UnitTests.TestHelpers;

public static class ModelStateDictionaryExtensions
{
    public static List<string> AllErrorMessages(this ModelStateDictionary @this)
    {
        return @this.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
    }
}