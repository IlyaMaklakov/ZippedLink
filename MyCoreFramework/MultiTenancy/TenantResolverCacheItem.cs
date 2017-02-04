namespace MyCoreFramework.MultiTenancy
{
    public class TenantResolverCacheItem
    {
        public int? TenantId { get; }

        public TenantResolverCacheItem(int? tenantId)
        {
            this.TenantId = tenantId;
        }
    }
}