using Microsoft.AspNetCore.Http;

using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;
using MyCoreFramework.MultiTenancy;

namespace MyCore.AspNetCore.MultiTenancy
{
    public class HttpCookieTenantResolveContributer : ITenantResolveContributer, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpCookieTenantResolveContributer(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public int? ResolveTenantId()
        {
            var httpContext = this._httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var tenantIdValue = httpContext.Request.Cookies[MultiTenancyConsts.TenantIdResolveKey];
            if (tenantIdValue.IsNullOrEmpty())
            {
                return null;
            }

            int tenantId;
            return !int.TryParse(tenantIdValue, out tenantId) ? (int?) null : tenantId;
        }
    }
}