using CMSprinkle.ViewModels;

namespace CMSprinkle.Tests.UnitTests.TestHelpers;

public static class EditContentSubmitModelHelper
{
    public static EditContentSubmitModel Create(string? content = null)
    {
        content ??= new Bogus.DataSets.Lorem().Sentences(5);

        var model = new EditContentSubmitModel();
        model.Content = content;
        return model;
    }
}