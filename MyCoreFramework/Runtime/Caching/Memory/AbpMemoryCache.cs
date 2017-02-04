using System;
using System.Runtime.Caching;

namespace MyCoreFramework.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICache"/> to work with <see cref="MemoryCache"/>.
    /// </summary>
    public class AbpMemoryCache : CacheBase
    {
        private MemoryCache _memoryCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Unique name of the cache</param>
        public AbpMemoryCache(string name)
            : base(name)
        {
            this._memoryCache = new MemoryCache(this.Name);
        }

        public override object GetOrDefault(string key)
        {
            return this._memoryCache.Get(key);
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            var cachePolicy = new CacheItemPolicy();

            if (absoluteExpireTime != null)
            {
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(absoluteExpireTime.Value);
            }
            else if (slidingExpireTime != null)
            {
                cachePolicy.SlidingExpiration = slidingExpireTime.Value;
            }
            else if(this.DefaultAbsoluteExpireTime != null)
            {
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(this.DefaultAbsoluteExpireTime.Value);
            }
            else
            {
                cachePolicy.SlidingExpiration = this.DefaultSlidingExpireTime;
            }

            this._memoryCache.Set(key, value, cachePolicy);
        }

        public override void Remove(string key)
        {
            this._memoryCache.Remove(key);
        }

        public override void Clear()
        {
            this._memoryCache.Dispose();
            this._memoryCache = new MemoryCache(this.Name);
        }

        public override void Dispose()
        {
            this._memoryCache.Dispose();
            base.Dispose();
        }
    }
}