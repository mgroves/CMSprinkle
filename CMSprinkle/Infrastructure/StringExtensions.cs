using Markdig;

namespace CMSprinkle.Infrastructure;

// all chatgpt generated
public static class StringExtensions
{
    public static string ToTruncateMarkdown(this string markdown, int maxLength)
    {
        // Convert Markdown to plain text
        string plainText = Markdown.ToPlainText(markdown ?? "");

        // Truncate the plain text
        return plainText.Length <= maxLength ? plainText : plainText.Substring(0, maxLength) + "...";
    }
}