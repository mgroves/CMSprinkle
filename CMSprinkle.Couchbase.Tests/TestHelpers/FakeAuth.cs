using CMSprinkle.Auth;

namespace CMSprinkle.Couchbase.Tests.TestHelpers;

public class FakeAuth : ICMSprinkleAuth
{
    public bool FakeIsAllowed = true;
    public string FakeGetUsername = "Anonymous";


    public async Task<bool> IsAllowed()
    {
        return FakeIsAllowed;
    }

    public async Task<string> GetUsername()
    {
        return FakeGetUsername;
    }
}