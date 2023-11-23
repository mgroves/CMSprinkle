using System.Threading.Tasks;

namespace CMSprinkle;

public interface ICMSprinkleAuth
{
    Task<bool> IsAllowed();
}