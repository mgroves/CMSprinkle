using CMSprinkle.Auth;

namespace CMSprinkle.Example;

// do NOT use this example
public class ExampleAuthClass : ICMSprinkleAuth
{
    public ExampleAuthClass()
    {
        // inject your auth dependencies in here
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