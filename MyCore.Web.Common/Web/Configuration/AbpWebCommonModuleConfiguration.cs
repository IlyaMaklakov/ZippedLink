using MyCore.Web.Api.ProxyScripting.Configuration;
using MyCore.Web.MultiTenancy;
using MyCore.Web.Security.AntiForgery;

namespace MyCore.Web.Configuration
{
    internal class AbpWebCommonModuleConfiguration : IAbpWebCommonModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public IAbpAntiForgeryConfiguration AntiForgery { get; }

        public IWebEmbeddedResourcesConfiguration EmbeddedResources { get; }

        public IWebMultiTenancyConfiguration MultiTenancy { get; }

        public AbpWebCommonModuleConfiguration(
            IApiProxyScriptingConfiguration apiProxyScripting, 
            IAbpAntiForgeryConfiguration abpAntiForgery,
            IWebEmbeddedResourcesConfiguration embeddedResources, 
            IWebMultiTenancyConfiguration multiTenancy)
        {
            this.ApiProxyScripting = apiProxyScripting;
            this.AntiForgery = abpAntiForgery;
            this.EmbeddedResources = embeddedResources;
            this.MultiTenancy = multiTenancy;
        }
    }
}