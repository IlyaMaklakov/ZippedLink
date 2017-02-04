using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

using MyCore.AspNetCore.Configuration;
using MyCore.AspNetCore.MultiTenancy;
using MyCore.AspNetCore.Mvc.Auditing;
using MyCore.AspNetCore.Runtime.Session;
using MyCore.AspNetCore.Security.AntiForgery;
using MyCore.Configuration.Startup;
using MyCore.Web;
using MyCore.Web.Security.AntiForgery;

using MyCoreFramework.Auditing;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Modules;
using MyCoreFramework.Runtime.Session;

namespace MyCore.AspNetCore
{
    [DependsOn(typeof(AbpWebCommonModule))]
    public class AbpAspNetCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            this.IocManager.Register<IAbpAspNetCoreConfiguration, AbpAspNetCoreConfiguration>();

            this.Configuration.ReplaceService<IPrincipalAccessor, AspNetCorePrincipalAccessor>(DependencyLifeStyle.Transient);
            this.Configuration.ReplaceService<IAbpAntiForgeryManager, AbpAspNetCoreAntiForgeryManager>(DependencyLifeStyle.Transient);
            this.Configuration.ReplaceService<IClientInfoProvider, HttpContextClientInfoProvider>(DependencyLifeStyle.Transient);

            this.Configuration.Modules.AbpAspNetCore().FormBodyBindingIgnoredTypes.Add(typeof(IFormFile));

            this.Configuration.MultiTenancy.Resolvers.Add<DomainTenantResolveContributer>();
            this.Configuration.MultiTenancy.Resolvers.Add<HttpHeaderTenantResolveContributer>();
            this.Configuration.MultiTenancy.Resolvers.Add<HttpCookieTenantResolveContributer>();
        }

        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            this.AddApplicationParts();
            this.ConfigureAntiforgery();
        }

        private void AddApplicationParts()
        {
            var configuration = this.IocManager.Resolve<AbpAspNetCoreConfiguration>();
            var partManager = this.IocManager.Resolve<ApplicationPartManager>();
            var moduleManager = this.IocManager.Resolve<IAbpModuleManager>();

            var controllerAssemblies = configuration.ControllerAssemblySettings.Select(s => s.Assembly).Distinct();
            foreach (var controllerAssembly in controllerAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
            }

            var plugInAssemblies = moduleManager.Modules.Where(m => m.IsLoadedAsPlugIn).Select(m => m.Assembly).Distinct();
            foreach (var plugInAssembly in plugInAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(plugInAssembly));
            }
        }

        private void ConfigureAntiforgery()
        {
            this.IocManager.Using<IOptions<AntiforgeryOptions>>(optionsAccessor =>
            {
                optionsAccessor.Value.HeaderName = this.Configuration.Modules.AbpWebCommon().AntiForgery.TokenHeaderName;
            });
        }
    }
}