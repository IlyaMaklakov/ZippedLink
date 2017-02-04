using System.Reflection;

using MyCore.Configuration.Startup;
using MyCore.Web.Api.ProxyScripting.Configuration;
using MyCore.Web.Api.ProxyScripting.Generators.JQuery;
using MyCore.Web.Configuration;
using MyCore.Web.MultiTenancy;
using MyCore.Web.Security.AntiForgery;

using MyCoreFramework;
using MyCoreFramework.Localization.Dictionaries;
using MyCoreFramework.Localization.Dictionaries.Xml;
using MyCoreFramework.Modules;

namespace MyCore.Web
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]    
    public class AbpWebCommonModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            this.IocManager.Register<IWebMultiTenancyConfiguration, WebMultiTenancyConfiguration>();
            this.IocManager.Register<IApiProxyScriptingConfiguration, ApiProxyScriptingConfiguration>();
            this.IocManager.Register<IAbpAntiForgeryConfiguration, AbpAntiForgeryConfiguration>();
            this.IocManager.Register<IWebEmbeddedResourcesConfiguration, WebEmbeddedResourcesConfiguration>();
            this.IocManager.Register<IAbpWebCommonModuleConfiguration, AbpWebCommonModuleConfiguration>();

            this.Configuration.Modules.AbpWebCommon().ApiProxyScripting.Generators[JQueryProxyScriptGenerator.Name] = typeof(JQueryProxyScriptGenerator);

            this.Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpWebConsts.LocalizaionSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(), "Abp.Web.Common.Web.Localization.AbpWebXmlSource"
                        )));
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }
    }
}
