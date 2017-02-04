namespace MyCore.Web.MultiTenancy
{
    public interface IWebMultiTenancyConfiguration
    {
        string DomainFormat { get; set; }
    }
}