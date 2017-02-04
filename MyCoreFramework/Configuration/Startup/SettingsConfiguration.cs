using MyCoreFramework.Collections;

namespace MyCoreFramework.Configuration.Startup
{
    internal class SettingsConfiguration : ISettingsConfiguration
    {
        public ITypeList<SettingProvider> Providers { get; private set; }

        public SettingsConfiguration()
        {
            this.Providers = new TypeList<SettingProvider>();
        }
    }
}