using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using MyCore.AspNetCore;
using MyCore.AspNetCore.Configuration;

using MyCoreFramework;
using MyCoreFramework.Modules;

using ZippedLink.Application;
using ZippedLink.Core.Configuration;
using ZippedLink.EntityFramework;

namespace ZippedLink.WebSpa.Startup
{
    [DependsOn(
    typeof(ZippedLinkApplicationModule),
    typeof(ZippedLinkDataModule),
    typeof(AbpAspNetCoreModule))]
    public class ZippedLinkWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public ZippedLinkWebModule(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(MyCoreConsts.ConnectionStringName);


            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(ZippedLinkApplicationModule).Assembly
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
