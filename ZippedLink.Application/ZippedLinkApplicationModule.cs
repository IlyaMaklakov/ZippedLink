using System.Reflection;

using MyCore.AutoMapper;

using MyCoreFramework.Modules;

using ZippedLink.Core;

namespace ZippedLink.Application
{
    [DependsOn(
    typeof(ZippedLinkCoreModule),
    typeof(AbpAutoMapperModule))]
    public class ZippedLinkApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
         
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
