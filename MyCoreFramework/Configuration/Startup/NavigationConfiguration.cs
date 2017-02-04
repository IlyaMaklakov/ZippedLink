using MyCoreFramework.Application.Navigation;
using MyCoreFramework.Collections;

namespace MyCoreFramework.Configuration.Startup
{
    internal class NavigationConfiguration : INavigationConfiguration
    {
        public ITypeList<NavigationProvider> Providers { get; private set; }

        public NavigationConfiguration()
        {
            this.Providers = new TypeList<NavigationProvider>();
        }
    }
}