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
}