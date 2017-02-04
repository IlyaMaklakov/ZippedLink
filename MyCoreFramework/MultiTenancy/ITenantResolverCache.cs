using MyCoreFramework.JetBrains.Annotations;

namespace MyCoreFramework.MultiTenancy
{
    public interface ITenantResolverCache
    {
        [CanBeNull]
        TenantResolverCacheItem Value { get; set; }
    }
}