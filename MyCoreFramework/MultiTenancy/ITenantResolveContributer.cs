namespace MyCoreFramework.MultiTenancy
{
    public interface ITenantResolveContributer
    {
        int? ResolveTenantId();
    }
}