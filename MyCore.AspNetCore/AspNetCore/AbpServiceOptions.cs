using MyCoreFramework.Dependency;
using MyCoreFramework.PlugIns;

namespace MyCore.AspNetCore
{
    public class AbpServiceOptions
    {
        public IIocManager IocManager { get; set; }

        public PlugInSourceList PlugInSources { get; }

        public AbpServiceOptions()
        {
            this.PlugInSources = new PlugInSourceList();
        }
    }
}