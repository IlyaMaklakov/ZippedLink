using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using MyCoreFramework.Configuration.Startup;

namespace MyCoreFramework.Runtime.Caching.Configuration
{
    internal class CachingConfiguration : ICachingConfiguration
    {
        public IAbpStartupConfiguration AbpConfiguration { get; private set; }

        public IReadOnlyList<ICacheConfigurator> Configurators
        {
            get { return this._configurators.ToImmutableList(); }
        }
        private readonly List<ICacheConfigurator> _configurators;

        public CachingConfiguration(IAbpStartupConfiguration abpConfiguration)
        {
            this.AbpConfiguration = abpConfiguration;

            this._configurators = new List<ICacheConfigurator>();
        }

        public void ConfigureAll(Action<ICache> initAction)
        {
            this._configurators.Add(new CacheConfigurator(initAction));
        }

        public void Configure(string cacheName, Action<ICache> initAction)
        {
            this._configurators.Add(new CacheConfigurator(cacheName, initAction));
        }
    }
}