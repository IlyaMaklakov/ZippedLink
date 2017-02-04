using Microsoft.AspNetCore.Http;

using MyCoreFramework.Dependency;
using MyCoreFramework.MultiTenancy;

namespace MyCore.AspNetCore.MultiTenancy
{
    public class HttpContextTenantResolverCache : ITenantResolverCache, ITransientDependency
    {
        private const string CacheItemKey = "Abp.MultiTenancy.TenantResolverCacheItem";

        public TenantResolverCacheItem Value
        {
            get
            {
                return this._httpContextAccessor.HttpContext?.Items[CacheItemKey] as TenantResolverCacheItem;
            }

            set
            {
                var httpContext = this._httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return;
                }

                httpContext.Items[CacheItemKey] = value;
            }
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTenantResolverCache(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
    }
}
