using System.Threading.Tasks;

using MyCoreFramework.Domain.Repositories;
using MyCoreFramework.Events.Bus.Entities;
using MyCoreFramework.Events.Bus.Handlers;
using MyCoreFramework.ObjectMapping;
using MyCoreFramework.Runtime.Caching;

namespace MyCoreFramework.Domain.Entities.Caching
{
    public class EntityCache<TEntity, TCacheItem> :
        EntityCache<TEntity, TCacheItem, int>,
        IEntityCache<TCacheItem>
        where TEntity : class, IEntity<int>
    {
        public EntityCache(
            ICacheManager cacheManager,
            IRepository<TEntity, int> repository,
            string cacheName = null)
            : base(
                cacheManager,
                repository,
                cacheName)
        {
        }
    }

    public class EntityCache<TEntity, TCacheItem, TPrimaryKey> :
        IEventHandler<EntityChangedEventData<TEntity>>, IEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public TCacheItem this[TPrimaryKey id]
        {
            get { return this.Get(id); }
        }

        public string CacheName { get; private set; }

        public ITypedCache<TPrimaryKey, TCacheItem> InternalCache
        {
            get
            {
                return this.CacheManager.GetCache<TPrimaryKey, TCacheItem>(this.CacheName);
            }
        }

        public IObjectMapper ObjectMapper { get; set; }

        protected ICacheManager CacheManager { get; private set; }

        protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }

        public EntityCache(
            ICacheManager cacheManager, 
            IRepository<TEntity, TPrimaryKey> repository, 
            string cacheName = null)
        {
            this.Repository = repository;
            this.CacheManager = cacheManager;
            this.CacheName = cacheName ?? this.GenerateDefaultCacheName();
            this.ObjectMapper = NullObjectMapper.Instance;
        }

        public virtual TCacheItem Get(TPrimaryKey id)
        {
            return this.InternalCache.Get(id, () => this.GetCacheItemFromDataSource(id));
        }

        public virtual Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            return this.InternalCache.GetAsync(id, () => this.GetCacheItemFromDataSourceAsync(id));
        }

        public virtual void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            this.InternalCache.Remove(eventData.Entity.Id);
        }

        protected virtual TCacheItem GetCacheItemFromDataSource(TPrimaryKey id)
        {
            return this.MapToCacheItem(this.GetEntityFromDataSource(id));
        }

        protected virtual async Task<TCacheItem> GetCacheItemFromDataSourceAsync(TPrimaryKey id)
        {
            return this.MapToCacheItem(await this.GetEntityFromDataSourceAsync(id));
        }

        protected virtual TEntity GetEntityFromDataSource(TPrimaryKey id)
        {
            return this.Repository.FirstOrDefault(id);
        }

        protected virtual Task<TEntity> GetEntityFromDataSourceAsync(TPrimaryKey id)
        {
            return this.Repository.FirstOrDefaultAsync(id);
        }

        protected virtual TCacheItem MapToCacheItem(TEntity entity)
        {
            if (this.ObjectMapper is NullObjectMapper)
            {
                throw new AbpException(
                    string.Format(
                        "MapToCacheItem method should be overrided or IObjectMapper should be implemented in order to map {0} to {1}",
                        typeof (TEntity),
                        typeof (TCacheItem)
                        )
                    );
            }

            return this.ObjectMapper.Map<TCacheItem>(entity);
        }

        protected virtual string GenerateDefaultCacheName()
        {
            return this.GetType().FullName;
        }

        public override string ToString()
        {
            return string.Format("EntityCache {0}", this.CacheName);
        }
    }
}
