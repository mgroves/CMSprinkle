namespace CMSprinkle.SqlServer.Tests.TestHelpers;

public static class ExceptionExtensions
{
    public static IEnumerable<Exception> AllExceptions(this Exception @this)
    {
        yield return @this; // Return the top-level exception

        if (@this.InnerException != null)
        {
            // Recursively yield all inner exceptions
            foreach (var innerException in @this.InnerException.AllExceptions())
            {
                yield return innerException;
            }
        }

        // If the exception is an AggregateException, handle its inner exceptions
        if (@this is AggregateException aggEx)
        {
            foreach (var innerEx in aggEx.InnerExceptions)
            {
                foreach (var innerInnerEx in innerEx.AllExceptions())
                {
                    yield return innerInnerEx;
                }
            }
        }
    }
}
