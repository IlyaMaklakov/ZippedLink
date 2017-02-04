using Microsoft.AspNetCore.Http;

using MyCore.Web.MultiTenancy;

using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;
using MyCoreFramework.MultiTenancy;
using MyCoreFramework.Text;

namespace MyCore.AspNetCore.MultiTenancy
{
    public class DomainTenantResolveContributer : ITenantResolveContributer, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebMultiTenancyConfiguration _multiTenancyConfiguration;

        public DomainTenantResolveContributer(
            IHttpContextAccessor httpContextAccessor, 
            IWebMultiTenancyConfiguration multiTenancyConfiguration)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._multiTenancyConfiguration = multiTenancyConfiguration;
        }

        public int? ResolveTenantId()
        {
            if (this._multiTenancyConfiguration.DomainFormat.IsNullOrEmpty())
            {
                return null;
            }

            var httpContext = this._httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var hostName = httpContext.Request.Host.Host.RemovePreFix("http://", "https://");
            var result = new FormattedStringValueExtracter().Extract(hostName, this._multiTenancyConfiguration.DomainFormat, true);
            if (!result.IsMatch)
            {
                return null;
            }

            var tenantIdValue = result.Matches[0].Value;
            if (tenantIdValue.IsNullOrEmpty())
            {
                return null;
            }

            int tenantId;
            return !int.TryParse(tenantIdValue, out tenantId) ? (int?)null : tenantId;
        }
    }
}