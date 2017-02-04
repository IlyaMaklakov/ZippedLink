using Microsoft.AspNetCore.Mvc;

using MyCore.AspNetCore.Mvc.Controllers;
using MyCore.Web.Api.ProxyScripting;

using MyCoreFramework.Auditing;
using MyCoreFramework.Web.Models;

namespace MyCore.AspNetCore.Mvc.Proxying
{
    [DontWrapResult]
    [DisableAuditing]
    public class AbpServiceProxiesController : AbpController
    {
        private readonly IApiProxyScriptManager _proxyScriptManager;

        public AbpServiceProxiesController(IApiProxyScriptManager proxyScriptManager)
        {
            this._proxyScriptManager = proxyScriptManager;
        }

        [Produces("text/javascript")]
        public string GetAll(ApiProxyGenerationModel model)
        {
            return this._proxyScriptManager.GetScript(model.CreateOptions());
        }
    }
}
