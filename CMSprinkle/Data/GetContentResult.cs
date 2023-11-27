using System;

namespace CMSprinkle.Data;

public class GetContentResult
{
    public static Func<string,string> ContentNotFoundMessage;

    public required string Key { get; set; }
    private string _content;
    public required string Content
    {
        get => string.IsNullOrEmpty(_content) ? ContentNotFoundMessage(Key) : _content;
        set => _content = value;
    }

    public required string LastUser { get; set; }
}