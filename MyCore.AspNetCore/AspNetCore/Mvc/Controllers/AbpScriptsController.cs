using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MyCore.Web.Authorization;
using MyCore.Web.Features;
using MyCore.Web.Localization;
using MyCore.Web.MultiTenancy;
using MyCore.Web.Navigation;
using MyCore.Web.Sessions;
using MyCore.Web.Settings;
using MyCore.Web.Timing;

using MyCoreFramework.Auditing;
using MyCoreFramework.Extensions;

namespace MyCore.AspNetCore.Mvc.Controllers
{
    /// <summary>
    /// This controller is used to create client side scripts
    /// to work with ABP.
    /// </summary>
    public class AbpScriptsController : AbpController
    {
        private readonly IMultiTenancyScriptManager _multiTenancyScriptManager;
        private readonly ISettingScriptManager _settingScriptManager;
        private readonly INavigationScriptManager _navigationScriptManager;
        private readonly ILocalizationScriptManager _localizationScriptManager;
        private readonly IAuthorizationScriptManager _authorizationScriptManager;
        private readonly IFeaturesScriptManager _featuresScriptManager;
        private readonly ISessionScriptManager _sessionScriptManager;
        private readonly ITimingScriptManager _timingScriptManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpScriptsController(
            IMultiTenancyScriptManager multiTenancyScriptManager,
            ISettingScriptManager settingScriptManager,
            INavigationScriptManager navigationScriptManager,
            ILocalizationScriptManager localizationScriptManager,
            IAuthorizationScriptManager authorizationScriptManager,
            IFeaturesScriptManager featuresScriptManager,
            ISessionScriptManager sessionScriptManager, 
            ITimingScriptManager timingScriptManager)
        {
            this._multiTenancyScriptManager = multiTenancyScriptManager;
            this._settingScriptManager = settingScriptManager;
            this._navigationScriptManager = navigationScriptManager;
            this._localizationScriptManager = localizationScriptManager;
            this._authorizationScriptManager = authorizationScriptManager;
            this._featuresScriptManager = featuresScriptManager;
            this._sessionScriptManager = sessionScriptManager;
            this._timingScriptManager = timingScriptManager;
        }

        /// <summary>
        /// Gets all needed scripts.
        /// </summary>
        [DisableAuditing]
        public async Task<ActionResult> GetScripts(string culture = "")
        {
            if (!culture.IsNullOrEmpty())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            }

            var sb = new StringBuilder();

            sb.AppendLine(this._multiTenancyScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(this._sessionScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(this._localizationScriptManager.GetScript());
            sb.AppendLine();

            sb.AppendLine(await this._featuresScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await this._authorizationScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await this._navigationScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await this._settingScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(await this._timingScriptManager.GetScriptAsync());
            sb.AppendLine();

            sb.AppendLine(GetTriggerScript());
            
            return this.Content(sb.ToString(), "application/x-javascript", Encoding.UTF8);
        }

        private static string GetTriggerScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    abp.event.trigger('abp.dynamicScriptsInitialized');");
            script.Append("})();");

            return script.ToString();
        }
    }
}
