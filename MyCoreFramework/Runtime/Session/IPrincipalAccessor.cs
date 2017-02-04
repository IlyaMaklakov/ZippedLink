using System.Security.Claims;

namespace MyCoreFramework.Runtime.Session
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}
