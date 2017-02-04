using System.Text;

using MyCore.Web.Security.AntiForgery;

using MyCoreFramework.Dependency;

namespace MyCore.Web.Security
{
    internal class SecurityScriptManager : ISecurityScriptManager, ITransientDependency
    {
        private readonly IAbpAntiForgeryConfiguration _abpAntiForgeryConfiguration;

        public SecurityScriptManager(IAbpAntiForgeryConfiguration abpAntiForgeryConfiguration)
        {
            this._abpAntiForgeryConfiguration = abpAntiForgeryConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    abp.security.antiForgery.tokenCookieName = '" + this._abpAntiForgeryConfiguration.TokenCookieName + "';");
            script.AppendLine("    abp.security.antiForgery.tokenHeaderName = '" + this._abpAntiForgeryConfiguration.TokenHeaderName + "';");
            script.Append("})();");

            return script.ToString();
        }
    }
}
