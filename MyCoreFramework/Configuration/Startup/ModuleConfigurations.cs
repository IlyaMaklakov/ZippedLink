namespace MyCoreFramework.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public IAbpStartupConfiguration AbpConfiguration { get; private set; }

        public ModuleConfigurations(IAbpStartupConfiguration abpConfiguration)
        {
            this.AbpConfiguration = abpConfiguration;
        }
    }
}