using CMSprinkle.Auth;

namespace CMSprinkle.Example;

// do NOT use this example
public class ExampleAuthClass : ICMSprinkleAuth
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExampleAuthClass(IHttpContextAccessor httpContextAccessor)
    {
        // inject your auth dependencies in here
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> IsAllowed()
    {
        // Put your auth code here
        // "true" means the use has access to CMSprinkle management tools (add/edit/etc)
        return true;
    }

    public async Task<string> GetUsername()
    {
        // put code here to get username
        // this will be saved in the content (last user to edit, for instance)
        return "user-" + Path.GetRandomFileName();
    }
}