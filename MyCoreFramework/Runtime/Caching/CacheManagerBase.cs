using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using MyCoreFramework.Dependency;
using MyCoreFramework.Runtime.Caching.Configuration;

namespace MyCoreFramework.Runtime.Caching
{
    /// <summary>
    /// Base class for cache managers.
    /// </summary>
    public abstract class CacheManagerBase : ICacheManager, ISingletonDependency
    {
        protected readonly IIocManager IocManager;

        protected readonly ICachingConfiguration Configuration;

        protected readonly ConcurrentDictionary<string, ICache> Caches;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="iocManager"></param>
        /// <param name="configuration"></param>
        protected CacheManagerBase(IIocManager iocManager, ICachingConfiguration configuration)
        {
            this.IocManager = iocManager;
            this.Configuration = configuration;
            this.Caches = new ConcurrentDictionary<string, ICache>();
        }

        public IReadOnlyList<ICache> GetAllCaches()
        {
            return this.Caches.Values.ToImmutableList();
        }
        
        public virtual ICache GetCache(string name)
        {
            Check.NotNull(name, nameof(name));

            return this.Caches.GetOrAdd(name, (cacheName) =>
            {
                var cache = this.CreateCacheImplementation(cacheName);

                var configurators = this.Configuration.Configurators.Where(c => c.CacheName == null || c.CacheName == cacheName);

                foreach (var configurator in configurators)
                {
                    configurator.InitAction?.Invoke(cache);
                }

                return cache;
            });
        }

        public virtual void Dispose()
        {
            foreach (var cache in this.Caches)
            {
                this.IocManager.Release(cache.Value);
            }

            this.Caches.Clear();
        }

        /// <summary>
        /// Used to create actual cache implementation.
        /// </summary>
        /// <param name="name">Name of the cache</param>
        /// <returns>Cache object</returns>
        protected abstract ICache CreateCacheImplementation(string name);
    }
}