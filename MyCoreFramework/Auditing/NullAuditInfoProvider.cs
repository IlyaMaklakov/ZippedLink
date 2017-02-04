using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;

namespace MyCoreFramework.Auditing
{
    /// <summary>
    /// Default implementation of <see cref="IAuditInfoProvider" />.
    /// </summary>
    public class DefaultAuditInfoProvider : IAuditInfoProvider, ITransientDependency
    {
        public IClientInfoProvider ClientInfoProvider { get; set; }

        public DefaultAuditInfoProvider()
        {
            this.ClientInfoProvider = NullClientInfoProvider.Instance;
        }

        public virtual void Fill(AuditInfo auditInfo)
        {
            if (auditInfo.ClientIpAddress.IsNullOrEmpty())
            {
                auditInfo.ClientIpAddress = this.ClientInfoProvider.ClientIpAddress;
            }

            if (auditInfo.BrowserInfo.IsNullOrEmpty())
            {
                auditInfo.BrowserInfo = this.ClientInfoProvider.BrowserInfo;
            }

            if (auditInfo.ClientName.IsNullOrEmpty())
            {
                auditInfo.ClientName = this.ClientInfoProvider.ComputerName;
            }
        }
    }
}