using System.Text;

using MyCoreFramework.Dependency;
using MyCoreFramework.Runtime.Session;

namespace MyCore.Web.Sessions
{
    public class SessionScriptManager : ISessionScriptManager, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public SessionScriptManager()
        {
            this.AbpSession = NullAbpSession.Instance;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();

            script.AppendLine("    abp.session = abp.session || {};");
            script.AppendLine("    abp.session.userId = " + (this.AbpSession.UserId.HasValue ? this.AbpSession.UserId.Value.ToString() : "null") + ";");
            script.AppendLine("    abp.session.tenantId = " + (this.AbpSession.TenantId.HasValue ? this.AbpSession.TenantId.Value.ToString() : "null") + ";");
            script.AppendLine("    abp.session.impersonatorUserId = " + (this.AbpSession.ImpersonatorUserId.HasValue ? this.AbpSession.ImpersonatorUserId.Value.ToString() : "null") + ";");
            script.AppendLine("    abp.session.impersonatorTenantId = " + (this.AbpSession.ImpersonatorTenantId.HasValue ? this.AbpSession.ImpersonatorTenantId.Value.ToString() : "null") + ";");
            script.AppendLine("    abp.session.multiTenancySide = " + ((int)this.AbpSession.MultiTenancySide) + ";");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}