using System;
using System.Threading.Tasks;

using Nito.AsyncEx;

namespace MyCoreFramework.Runtime.Caching
{
    /// <summary>
    /// Base class for caches.
    /// It's used to simplify implementing <see cref="ICache"/>.
    /// </summary>
    public abstract class CacheBase : ICache
    {
        public string Name { get; }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        protected readonly object SyncObj = new object();

        private readonly AsyncLock _asyncLock = new AsyncLock();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected CacheBase(string name)
        {
            this.Name = name;
            this.DefaultSlidingExpireTime = TimeSpan.FromHours(1);
        }

        public virtual object Get(string key, Func<string, object> factory)
        {
            var cacheKey = key;
            var item = this.GetOrDefault(key);
            if (item == null)
            {
                lock (this.SyncObj)
                {
                    item = this.GetOrDefault(key);
                    if (item == null)
                    {
                        item = factory(key);
                        if (item == null)
                        {
                            return null;
                        }

                        this.Set(cacheKey, item);
                    }
                }
            }

            return item;
        }

        public virtual async Task<object> GetAsync(string key, Func<string, Task<object>> factory)
        {
            var cacheKey = key;
            var item = await this.GetOrDefaultAsync(key);
            if (item == null)
            {
                using (await this._asyncLock.LockAsync())
                {
                    item = await this.GetOrDefaultAsync(key);
                    if (item == null)
                    {
                        item = await factory(key);
                        if (item == null)
                        {
                            return null;
                        }

                        await this.SetAsync(cacheKey, item);
                    }
                }
            }

            return item;
        }

        public abstract object GetOrDefault(string key);

        public virtual Task<object> GetOrDefaultAsync(string key)
        {
            return Task.FromResult(this.GetOrDefault(key));
        }

        public abstract void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        public virtual Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            this.Set(key, value, slidingExpireTime);
            return Task.FromResult(0);
        }

        public abstract void Remove(string key);

        public virtual Task RemoveAsync(string key)
        {
            this.Remove(key);
            return Task.FromResult(0);
        }

        public abstract void Clear();

        public virtual Task ClearAsync()
        {
            this.Clear();
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {

        }
    }
}