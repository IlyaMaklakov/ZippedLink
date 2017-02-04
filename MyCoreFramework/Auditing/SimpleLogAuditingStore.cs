using System.Threading.Tasks;

using Castle.Core.Logging;

namespace MyCoreFramework.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditingStore"/> to simply write audits to logs.
    /// </summary>
    public class SimpleLogAuditingStore : IAuditingStore
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static SimpleLogAuditingStore Instance { get; } = new SimpleLogAuditingStore();

        public ILogger Logger { get; set; }

        public SimpleLogAuditingStore()
        {
            this.Logger = NullLogger.Instance;
        }

        public Task SaveAsync(AuditInfo auditInfo)
        {
            if (auditInfo.Exception == null)
            {
                this.Logger.Info(auditInfo.ToString());
            }
            else
            {
                this.Logger.Warn(auditInfo.ToString());
            }

            return Task.FromResult(0);
        }
    }
}