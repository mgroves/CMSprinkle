using CMSprinkle.ViewModels;

namespace CMSprinkle.Tests.UnitTests.TestHelpers;

public static class CMSprinkleHomeHelper
{
    public static CMSprinkleHome Create(int? allContentSize = null)
    {
        var model = new CMSprinkleHome();
        model.AllContent = CMSprinkleContentHelper.CreateList(allContentSize ?? 5);
        return model;
    }
}