using System;
using Newtonsoft.Json;

namespace CMSprinkle.Infrastructure;

public class CMSprinkleContent
{
    [JsonIgnore]
    public required string ContentKey { get; set; }
    public required string Content { get; set; }
    public required string LastUser { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedLast { get; set; }
}