using System;

namespace MyCoreFramework.Runtime.Caching.Configuration
{
    internal class CacheConfigurator : ICacheConfigurator
    {
        public string CacheName { get; private set; }

        public Action<ICache> InitAction { get; private set; }

        public CacheConfigurator(Action<ICache> initAction)
        {
            this.InitAction = initAction;
        }

        public CacheConfigurator(string cacheName, Action<ICache> initAction)
        {
            this.CacheName = cacheName;
            this.InitAction = initAction;
        }
    }
}