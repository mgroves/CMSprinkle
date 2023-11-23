using System.Threading.Tasks;

namespace CMSprinkle.Couchbase;

public interface ICMSprinkleAuth
{
    Task<bool> IsAllowed();
}