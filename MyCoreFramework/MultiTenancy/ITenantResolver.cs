namespace MyCoreFramework.MultiTenancy
{
    public interface ITenantResolver
    {
        int? ResolveTenantId();
    }
}