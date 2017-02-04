namespace MyCoreFramework.MultiTenancy
{
    public class TenantInfo
    {
        public int Id { get; set; }

        public string TenancyName { get; set; }

        public TenantInfo(int id, string tenancyName)
        {
            this.Id = id;
            this.TenancyName = tenancyName;
        }
    }
}