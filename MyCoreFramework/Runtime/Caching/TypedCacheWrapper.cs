using System;
using System.Threading.Tasks;

namespace MyCoreFramework.Runtime.Caching
{
    /// <summary>
    /// Implements <see cref="ITypedCache{TKey,TValue}"/> to wrap a <see cref="ICache"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TypedCacheWrapper<TKey, TValue> : ITypedCache<TKey, TValue>
    {
        public string Name
        {
            get { return this.InternalCache.Name; }
        }

        public TimeSpan DefaultSlidingExpireTime
        {
            get { return this.InternalCache.DefaultSlidingExpireTime; }
            set { this.InternalCache.DefaultSlidingExpireTime = value; }
        }

        public ICache InternalCache { get; private set; }

        /// <summary>
        /// Creates a new <see cref="TypedCacheWrapper{TKey,TValue}"/> object.
        /// </summary>
        /// <param name="internalCache">The actual internal cache</param>
        public TypedCacheWrapper(ICache internalCache)
        {
            this.InternalCache = internalCache;
        }

        public void Dispose()
        {
            this.InternalCache.Dispose();
        }

        public void Clear()
        {
            this.InternalCache.Clear();
        }

        public Task ClearAsync()
        {
            return this.InternalCache.ClearAsync();
        }

        public TValue Get(TKey key, Func<TKey, TValue> factory)
        {
            return this.InternalCache.Get(key, factory);
        }

        public Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            return this.InternalCache.GetAsync(key, factory);
        }

        public TValue GetOrDefault(TKey key)
        {
            return this.InternalCache.GetOrDefault<TKey, TValue>(key);
        }

        public Task<TValue> GetOrDefaultAsync(TKey key)
        {
            return this.InternalCache.GetOrDefaultAsync<TKey, TValue>(key);
        }

        public void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null)
        {
            this.InternalCache.Set(key.ToString(), value, slidingExpireTime);
        }

        public Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null)
        {
            return this.InternalCache.SetAsync(key.ToString(), value, slidingExpireTime);
        }

        public void Remove(TKey key)
        {
            this.InternalCache.Remove(key.ToString());
        }

        public Task RemoveAsync(TKey key)
        {
            return this.InternalCache.RemoveAsync(key.ToString());
        }
    }
}