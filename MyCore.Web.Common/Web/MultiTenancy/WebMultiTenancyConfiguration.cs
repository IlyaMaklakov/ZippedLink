namespace MyCore.Web.MultiTenancy
{
    public class WebMultiTenancyConfiguration : IWebMultiTenancyConfiguration
    {
        public string DomainFormat { get; set; }
    }
}