using System.Linq;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Runtime;

namespace MyCoreFramework.MultiTenancy
{
    public class TenantResolver : ITenantResolver, ITransientDependency
    {
        private const string AmbientScopeContextKey = "Abp.MultiTenancy.TenantResolver.Resolving";

        private readonly IMultiTenancyConfig _multiTenancy;
        private readonly IIocResolver _iocResolver;
        private readonly ITenantStore _tenantStore;
        private readonly ITenantResolverCache _cache;
        private readonly IAmbientScopeProvider<bool> _ambientScopeProvider;

        public TenantResolver(
            IMultiTenancyConfig multiTenancy,
            IIocResolver iocResolver,
            ITenantStore tenantStore,
            ITenantResolverCache cache,
            IAmbientScopeProvider<bool> ambientScopeProvider)
        {
            this._multiTenancy = multiTenancy;
            this._iocResolver = iocResolver;
            this._tenantStore = tenantStore;
            this._cache = cache;
            this._ambientScopeProvider = ambientScopeProvider;
        }

        public int? ResolveTenantId()
        {
            if (!this._multiTenancy.Resolvers.Any())
            {
                return null;
            }

            if (this._ambientScopeProvider.GetValue(AmbientScopeContextKey))
            {
                //Preventing recursive call of ResolveTenantId
                return null;
            }

            using (this._ambientScopeProvider.BeginScope(AmbientScopeContextKey, true))
            {
                var cacheItem = this._cache.Value;
                if (cacheItem != null)
                {
                    return cacheItem.TenantId;
                }

                var tenantId = this.GetTenantIdFromContributors();
                this._cache.Value = new TenantResolverCacheItem(tenantId);
                return tenantId;
            }
        }

        private int? GetTenantIdFromContributors()
        {
            foreach (var resolverType in this._multiTenancy.Resolvers)
            {
                using (var resolver = this._iocResolver.ResolveAsDisposable<ITenantResolveContributer>(resolverType))
                {
                    var tenantId = resolver.Object.ResolveTenantId();
                    if (tenantId == null)
                    {
                        continue;
                    }

                    if (this._tenantStore.Find(tenantId.Value) == null)
                    {
                        continue;
                    }

                    return tenantId;
                }
            }

            return null;
        }
    }
}