using System;

namespace CMSprinkle.Data;

public class GetContentResult
{
    public static Func<string,string> ContentNotFoundMessage;

    public required string Key { get; set; }
    private string _content;
    public required string Content
    {
        get
        {
            if (!string.IsNullOrEmpty(_content))
                return _content;
            if (ContentNotFoundMessage != null)
                return ContentNotFoundMessage(Key);
            return DefaultNotFoundMessage(Key);
        }
        set => _content = value;
    }

    public static string DefaultNotFoundMessage(string key)
    {
        return $"ERROR: Content Not Found ({key})";
    }
}