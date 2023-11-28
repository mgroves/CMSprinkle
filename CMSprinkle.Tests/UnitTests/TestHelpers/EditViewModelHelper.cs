using CMSprinkle.ViewModels;

namespace CMSprinkle.Tests.UnitTests.TestHelpers;

public static class EditViewModelHelper
{
    public static EditViewModel Create(string? contentKey = null, string? content = null, bool forceKeyNull = false)
    {
        if (!forceKeyNull)
            contentKey ??= "key-" + Path.GetRandomFileName();
        content ??= new Bogus.DataSets.Lorem().Sentences(5);

        var model = new EditViewModel();
        model.Content = content;
        model.Key = contentKey;
        return model;
    }
}