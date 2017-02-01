using System;
using System.Configuration;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;

namespace MyCoreFramework.Domain.Uow
{
    /// <summary>
    /// Default implementation of <see cref="IConnectionStringResolver"/>.
    /// Get connection string from <see cref="IStartupConfiguration"/>,
    /// or "Default" connection string in config file,
    /// or single connection string in config file.
    /// </summary>
    public class DefaultConnectionStringResolver : IConnectionStringResolver, ITransientDependency
    {
        private readonly IStartupConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConnectionStringResolver"/> class.
        /// </summary>
        public DefaultConnectionStringResolver(IStartupConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public virtual string GetNameOrConnectionString(ConnectionStringResolveArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var defaultConnectionString = this.configuration.DefaultNameOrConnectionString;
            if (!string.IsNullOrWhiteSpace(defaultConnectionString))
            {
                return defaultConnectionString;
            }

            if (ConfigurationManager.ConnectionStrings["Default"] != null)
            {
                return "Default";
            }

            if (ConfigurationManager.ConnectionStrings.Count == 1)
            {
                return ConfigurationManager.ConnectionStrings[0].ConnectionString;
            }

            throw new MyCoreException("Could not find a connection string definition for the application. Set IAbpStartupConfiguration.DefaultNameOrConnectionString or add a 'Default' connection string to application .config file.");
        }
    }
}