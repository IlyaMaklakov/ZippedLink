namespace MyCoreFramework.Runtime.Session
{
    public class SessionOverride
    {
        public long? UserId { get; }

        public int? TenantId { get; }

        public SessionOverride(int? tenantId, long? userId)
        {
            this.TenantId = tenantId;
            this.UserId = userId;
        }
    }
}