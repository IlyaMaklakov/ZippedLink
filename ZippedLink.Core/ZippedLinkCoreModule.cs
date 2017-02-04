using System.Reflection;

using MyCoreFramework.Modules;

namespace ZippedLink.Core
{
    public class ZippedLinkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
