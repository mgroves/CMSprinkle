using CMSprinkle.Infrastructure;
using NUnit.Framework;

namespace CMSprinkle.Tests.UnitTests.Infrastructure.StringExtensionsTests;

[TestFixture]
public class ToTruncateMarkdownTests
{
    [Test]
    public async Task Markdown_is_truncated_to_X_characters_plus_ellipsis()
    {
        // arrange
        var expectedSize = 50;
        var lengthOfEllipsis = 3;
        var markdown = new Bogus.DataSet().Random.Words(100);

        // act
        var result = markdown.ToTruncateMarkdown(expectedSize);

        // assert
        Assert.That(result.Length, Is.EqualTo(expectedSize + lengthOfEllipsis));
    }

    [Test]
    public async Task Markdown_is_stripped_to_plain_text()
    {
        // arrange
        var markdown = @"# header with **bold** text";

        // act
        var result = markdown.ToTruncateMarkdown(int.MaxValue);

        // assert
        Assert.That(result, Is.EqualTo("header with bold text\n"));
    }

    [Test]
    public async Task No_ellipsis_are_added_if_stripped_down_is_less_that_max()
    {
        // arrange
        var truncateLength = 50;
        var markdigAddsALineReturn = "\n";
        var markdown = new string('x', truncateLength-1);

        // act
        var result = markdown.ToTruncateMarkdown(truncateLength);

        // assert
        Assert.That(result, Is.EqualTo(markdown + markdigAddsALineReturn));
    }
}