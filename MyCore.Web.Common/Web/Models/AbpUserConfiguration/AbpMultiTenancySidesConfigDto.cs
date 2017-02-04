using MyCoreFramework.MultiTenancy;

namespace MyCore.Web.Models.AbpUserConfiguration
{
    public class AbpMultiTenancySidesConfigDto
    {
        public MultiTenancySides Host { get; private set; }

        public MultiTenancySides Tenant { get; private set; }

        public AbpMultiTenancySidesConfigDto()
        {
            this.Host = MultiTenancySides.Host;
            this.Tenant = MultiTenancySides.Tenant;
        }
    }
}