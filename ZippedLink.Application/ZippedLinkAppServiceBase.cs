using MyCoreFramework.Application.Services;

using ZippedLink.Core;

namespace ZippedLink.Application
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ZippedLinkAppServiceBase : ApplicationService
    {

        protected ZippedLinkAppServiceBase()
        {
            LocalizationSourceName = ZippedLinkConsts.LocalizationSourceName;
        }
    }
}
