using CMSprinkle.ViewModels;

namespace CMSprinkle.Tests.UnitTests.TestHelpers;

public static class AddContentSubmitModelHelper
{
    public static AddContentSubmitModel Create(string? key = null, string? content = null, bool forceKeyNull = false)
    {
        if(!forceKeyNull)
            key ??= "key-" + Path.GetRandomFileName();
        content ??= new Bogus.DataSets.Lorem().Sentences(5);

        var model = new AddContentSubmitModel();
        model.Key = key;
        model.Content = content;
        return model;
    }
}