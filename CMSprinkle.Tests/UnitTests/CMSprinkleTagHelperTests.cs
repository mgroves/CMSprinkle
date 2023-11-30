using CMSprinkle.Data;
using FakeItEasy;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;

namespace CMSprinkle.Tests.UnitTests;

[TestFixture]
public class CMSprinkleTagHelperTests
{
    private ICMSprinkleDataService _mockDataService;
    private CMSprinkleTagHelper _tagHelper;
    private TagHelperContext _tagHelperContext;
    private TagHelperOutput _tagHelperOutput;

    [SetUp]
    public async Task Setup()
    {
        _mockDataService = A.Fake<ICMSprinkleDataService>();
        _tagHelper = new CMSprinkleTagHelper(_mockDataService);

        // from https://www.hossambarakat.net/2016/02/29/unit-testing-asp-net-core-tag-helper/
        _tagHelperContext = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        _tagHelperOutput = new TagHelperOutput("something",
            new TagHelperAttributeList(), (_, _) => null);
            // {
            //     // this doesn't seem to be used
            //     var tagHelperContent = new DefaultTagHelperContent();
            //     tagHelperContent.SetHtmlContent(string.Empty);
            //     return Task.CompletedTask<TagHelperContent>();
            //     return Task.FromResult<TagHelperContent>(tagHelperContent);
            // });
    }

    [Test]
    public async Task Content_is_retrieved_and_displayed()
    {
        // arrange
        var onlyPlainTextContent = new GetContentResult
        {
            Key = "doesntmatter",
            Content = "lorem ipsum"
        };
        A.CallTo(() => _mockDataService.Get(A<string>._))
            .Returns(onlyPlainTextContent);

        // act
        await _tagHelper.ProcessAsync(_tagHelperContext, _tagHelperOutput);

        // assert
        var content = _tagHelperOutput.Content.GetContent();
        Assert.That(content, Does.Contain(onlyPlainTextContent.Content));
    }

    [TestCase("# This is markdown", "<h1>This is markdown</h1>")]
    [TestCase("*This is markdown*", "<em>This is markdown</em>")]
    [TestCase("[link](http://example.com)", "<a href=\"http://example.com\">link</a>")]
    public async Task Markdown_is_rendered_to_html(string markdown, string expectedHtml)
    {
        // arrange
        var onlyPlainTextContent = new GetContentResult
        {
            Key = "doesntmatter",
            Content = markdown
        };
        A.CallTo(() => _mockDataService.Get(A<string>._))
            .Returns(onlyPlainTextContent);

        // act
        await _tagHelper.ProcessAsync(_tagHelperContext, _tagHelperOutput);

        // assert
        var content = _tagHelperOutput.Content.GetContent();
        Assert.That(content, Does.Contain(expectedHtml));
    }

    // testing a few XSS strings just to make the point
    // I would love a way to throw a whole bunch of OWASP XSS strings at this, but
    // at some point I have to rely on HtmlSanitizer
    [TestCase("[link](javascript:alert('xss'))", "<p><a>link</a></p>\n")]
    [TestCase("<img src='http://url.to.file.which/not.exist' onerror=alert(document.cookie);>", "<img src=\"http://url.to.file.which/not.exist\">\n")]
    [TestCase("<img src='http://url.to.file.which/not.exist' onerror=alert(/xss/);>", "<img src=\"http://url.to.file.which/not.exist\">\n")]
    [TestCase("<img src=x onerror=\"alert('xss')\">", "<img src=\"x\">\n")]
    [TestCase("<body onload=alert('xss')> <p>hey</p>", " <p>hey</p>\n")]
    [TestCase("<input type=\"text\" value=``<clear=\"all\" style=\"background: url(javascript:alert('xss'))\">", "<p>&lt;input type=\"text\" value=``&lt;clear=\"all\" style=\"background: url(javascript:alert('xss'))\"&gt;</p>\n")]
    [TestCase("<SCRIPT SRC=https://cdn.jsdelivr.net/gh/Moksh45/host-xss.rocks/index.js></SCRIPT>", "\n")]
    public async Task Generated_HTML_is_sanitized_to_help_prevent_XSS(string markdown, string expectedHtml)
    {
        // arrange
        var onlyPlainTextContent = new GetContentResult
        {
            Key = "doesntmatter",
            Content = markdown
        };
        A.CallTo(() => _mockDataService.Get(A<string>._))
            .Returns(onlyPlainTextContent);

        // act
        await _tagHelper.ProcessAsync(_tagHelperContext, _tagHelperOutput);

        // assert
        var content = _tagHelperOutput.Content.GetContent();
        Assert.That(content, Is.EqualTo(expectedHtml));
    }
}