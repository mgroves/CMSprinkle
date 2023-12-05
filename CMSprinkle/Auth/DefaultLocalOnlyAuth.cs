using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CMSprinkle.Auth;

internal class DefaultLocalOnlyAuth : ICMSprinkleAuth
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DefaultLocalOnlyAuth(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> IsAllowed()
    {
        return IsLocalRequest(_httpContextAccessor.HttpContext);
    }

    public async Task<string> GetUsername()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Anonymous";
    }

    // from: https://stackoverflow.com/questions/35240586/in-asp-net-core-how-do-you-check-if-request-is-local
    private static bool IsLocalRequest(HttpContext context)
    {
        if (context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress))
        {
            return true;
        }
        if (IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
        {
            return true;
        }
        return false;
    }
}