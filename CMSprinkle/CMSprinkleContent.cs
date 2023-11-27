using Newtonsoft.Json;

namespace CMSprinkle;

public class CMSprinkleContent
{
    [JsonIgnore]
    public required string ContentKey { get; set; }
    public required string Content { get; set; }
    public required string LastUser { get; set; }
}