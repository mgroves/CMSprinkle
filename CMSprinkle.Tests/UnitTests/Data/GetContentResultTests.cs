using CMSprinkle.Data;
using NUnit.Framework;

namespace CMSprinkle.Tests.UnitTests.Data;

[TestFixture]
public class GetContentResultTests
{
    [Test]
    public async Task Use_content_not_found_message_if_no_content()
    {
        // arrange
        GetContentResult.ContentNotFoundMessage = s => $"expected message {s}";
        var getContentResult = new GetContentResult { Content = null, Key = "key" };

        // act
        var content = getContentResult.Content;

        // assert
        Assert.That(content, Is.EqualTo($"expected message key"));
    }

    [Test]
    public async Task Use_content_when_theres_content()
    {
        // arrange
        var expectedContent = new Bogus.DataSets.Hacker().Phrase();
        GetContentResult.ContentNotFoundMessage = s => $"expected message {s}";
        var getContentResult = new GetContentResult { Content = expectedContent, Key = "key" };

        // act
        var content = getContentResult.Content;

        // assert
        Assert.That(content, Is.EqualTo(expectedContent));
    }

    [Test]
    public async Task Use_default_message_if_ContentNotFoundMessage_is_set_to_null()
    {
        // arrange
        GetContentResult.ContentNotFoundMessage = null;
        var expectedMessage = GetContentResult.DefaultNotFoundMessage("key");
        var getContentResult = new GetContentResult { Content = null, Key = "key" };

        // act
        var content = getContentResult.Content;

        // assert
        Assert.That(content, Is.EqualTo(expectedMessage));
    }
}