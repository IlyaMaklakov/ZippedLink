using System.Linq;

using Castle.Core.Logging;

using Microsoft.AspNetCore.Http;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;
using MyCoreFramework.MultiTenancy;

namespace MyCore.AspNetCore.MultiTenancy
{
    public class HttpHeaderTenantResolveContributer : ITenantResolveContributer, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpHeaderTenantResolveContributer(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;

            this.Logger = NullLogger.Instance;
        }

        public int? ResolveTenantId()
        {
            var httpContext = this._httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var tenantIdHeader = httpContext.Request.Headers[MultiTenancyConsts.TenantIdResolveKey];
            if (tenantIdHeader == string.Empty || tenantIdHeader.Count < 1)
            {
                return null;
            }

            if (tenantIdHeader.Count > 1)
            { 
                this.Logger.Warn(
                    $"HTTP request includes more than one {MultiTenancyConsts.TenantIdResolveKey} header value. First one will be used. All of them: {tenantIdHeader.JoinAsString(", ")}"
                    );
            }

            int tenantId;
            return !int.TryParse(tenantIdHeader.First(), out tenantId) ? (int?) null : tenantId;
        }
    }
}
