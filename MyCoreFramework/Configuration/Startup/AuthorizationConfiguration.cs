using MyCoreFramework.Authorization;
using MyCoreFramework.Collections;

namespace MyCoreFramework.Configuration.Startup
{
    internal class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public ITypeList<AuthorizationProvider> Providers { get; }

        public bool IsEnabled { get; set; }

        public AuthorizationConfiguration()
        {
            this.Providers = new TypeList<AuthorizationProvider>();
            this.IsEnabled = true;
        }
    }
}