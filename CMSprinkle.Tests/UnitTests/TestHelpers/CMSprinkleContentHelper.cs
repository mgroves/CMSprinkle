using CMSprinkle.Infrastructure;

namespace CMSprinkle.Tests.UnitTests.TestHelpers;

public static class CMSprinkleContentHelper
{
    public static CMSprinkleContent Create(
        string? contentKey = null,
        string? content = null,
        DateTimeOffset? createdAt = null,
        string? lastUser = null,
        DateTimeOffset? updatedLast = null)
    {
        contentKey ??= "key-" + Path.GetRandomFileName();
        content ??= new Bogus.DataSets.Lorem().Sentences(5);
        createdAt ??= new Bogus.DataSets.Date().RecentOffset();
        updatedLast ??= new Bogus.DataSets.Date().RecentOffset();
        lastUser ??= new Bogus.DataSets.Internet().UserName();

        var model = new CMSprinkleContent
        {
            ContentKey = contentKey,
            Content = content,
            CreatedAt = createdAt.Value,
            LastUser = lastUser,
            UpdatedLast = updatedLast.Value
        };
        return model;
    }

    public static List<CMSprinkleContent> CreateList(int? size = null)
    {
        var rand = new Random();
        size ??= rand.Next(1, 6);

        var list = new List<CMSprinkleContent>();
        for (var i = 0; i < size; i++)
            list.Add(CMSprinkleContentHelper.Create());
        return list;
    }
}