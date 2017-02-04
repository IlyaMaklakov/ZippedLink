using MyCoreFramework.Collections;
using MyCoreFramework.MultiTenancy;

namespace MyCoreFramework.Configuration.Startup
{
    /// <summary>
    /// Used to configure multi-tenancy.
    /// </summary>
    internal class MultiTenancyConfig : IMultiTenancyConfig
    {
        /// <summary>
        /// Is multi-tenancy enabled?
        /// Default value: false.
        /// </summary>
        public bool IsEnabled { get; set; }

        public ITypeList<ITenantResolveContributer> Resolvers { get; }

        public MultiTenancyConfig()
        {
            this.Resolvers = new TypeList<ITenantResolveContributer>();
        }
    }
}