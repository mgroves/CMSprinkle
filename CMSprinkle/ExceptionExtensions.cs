using System.Text;
using System;

namespace CMSprinkle;

public static class ExceptionExtensions
{
    public static string GetAllExceptionMessages(this Exception @this)
    {
        var message = new StringBuilder();

        while (@this != null)
        {
            if (message.Length > 0)
                message.AppendLine(); // Add a line break between messages

            message.Append(@this.Message);
            @this = @this.InnerException;
        }

        return message.ToString();
    }

}