namespace MyCore.Web.Models.AbpUserConfiguration
{
    public class AbpMultiTenancyConfigDto
    {
        public bool IsEnabled { get; set; }

        public AbpMultiTenancySidesConfigDto Sides { get; private set; }

        public AbpMultiTenancyConfigDto()
        {
            this.Sides = new AbpMultiTenancySidesConfigDto();
        }
    }
}