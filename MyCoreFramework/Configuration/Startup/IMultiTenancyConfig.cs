using MyCoreFramework.Collections;
using MyCoreFramework.MultiTenancy;

namespace MyCoreFramework.Configuration.Startup
{
    /// <summary>
    /// Used to configure multi-tenancy.
    /// </summary>
    public interface IMultiTenancyConfig
    {
        /// <summary>
        /// Is multi-tenancy enabled?
        /// Default value: false.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// A list of contributers for tenant resolve process.
        /// </summary>
        ITypeList<ITenantResolveContributer> Resolvers { get; }
    }
}
