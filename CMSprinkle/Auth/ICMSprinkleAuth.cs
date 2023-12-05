using System.Threading.Tasks;

namespace CMSprinkle.Auth;

public interface ICMSprinkleAuth
{
    Task<bool> IsAllowed();
    Task<string> GetUsername();
}