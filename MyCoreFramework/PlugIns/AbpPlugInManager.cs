namespace MyCoreFramework.PlugIns
{
    public class AbpPlugInManager : IAbpPlugInManager
    {
        public PlugInSourceList PlugInSources { get; }

        public AbpPlugInManager()
        {
            this.PlugInSources = new PlugInSourceList();
        }
    }
}