using System.Threading.Tasks;
using CMSprinkle.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CMSprinkle;

[HtmlTargetElement("CMSprinkle")]
public class CMSprinkleTagHelper : TagHelper
{
    private readonly ICMSprinkleDataService _dataService;

    [HtmlAttributeName("contentKey")]
    public string ContentKey { get; set; }

    public CMSprinkleTagHelper(ICMSprinkleDataService dataService)
    {
        _dataService = dataService;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await _dataService.Get(ContentKey);
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Content.SetHtmlContent(content.Content);
    }
}