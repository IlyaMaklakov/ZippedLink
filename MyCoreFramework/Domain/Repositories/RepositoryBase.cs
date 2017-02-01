using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Entities;
using MyCoreFramework.MultiTenancy;
using MyCoreFramework.Reflection.Extensions;

namespace MyCoreFramework.Domain.Repositories
{
    /// <summary>
    /// Base class to implement <see cref="IRepository{TEntity,TPrimaryKey}"/>.
    /// It implements some methods in most simple way.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// The multi tenancy side
        /// </summary>
        public static MultiTenancySides? MultiTenancySide { get; private set; }

        public IIocResolver IocResolver { get; set; }

        static RepositoryBase()
        {
            var attr = typeof (TEntity).GetSingleAttributeOfTypeOrBaseTypesOrNull<MultiTenancySideAttribute>();
            if (attr != null)
            {
                MultiTenancySide = attr.Side;
            }
        }

        public abstract IQueryable<TEntity> GetAll();

        public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return this.GetAll();
        }

        public virtual List<TEntity> GetAllList()
        {
            return this.GetAll().ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync()
        {
            return Task.FromResult(this.GetAllList());
        }
        
        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return this.GetAll().Where(predicate).ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(this.GetAllList(predicate));
        }

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(this.GetAll());
        }
        
        public virtual TEntity Get(TPrimaryKey id)
        {
            var entity = this.FirstOrDefault(id);
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            var entity = await this.FirstOrDefaultAsync(id);
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }
        
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return this.GetAll().Single(predicate);
        }

        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(this.Single(predicate));
        }
        
        public virtual TEntity FirstOrDefault(TPrimaryKey id)
        {
            return this.GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return Task.FromResult(this.FirstOrDefault(id));
        }
        
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return this.GetAll().FirstOrDefault(predicate);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(this.FirstOrDefault(predicate));
        }
        
        public virtual TEntity Load(TPrimaryKey id)
        {
            return this.Get(id);
        }

        public abstract TEntity Insert(TEntity entity);
        
        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(this.Insert(entity));
        }

        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            return this.Insert(entity).Id;
        }

        public virtual Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(this.InsertAndGetId(entity));
        }

        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            return entity.IsTransient()
                ? this.Insert(entity)
                : this.Update(entity);
        }
        
        public virtual async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return entity.IsTransient()
                ? await this.InsertAsync(entity)
                : await this.UpdateAsync(entity);
        }

        public virtual TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            return this.InsertOrUpdate(entity).Id;
        }

        public virtual Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(this.InsertOrUpdateAndGetId(entity));
        }

        public abstract TEntity Update(TEntity entity);
        
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(this.Update(entity));
        }

        public virtual TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            var entity = this.Get(id);
            updateAction(entity);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction)
        {
            var entity = await this.GetAsync(id);
            await updateAction(entity);
            return entity;
        }

        public abstract void Delete(TEntity entity);
        
        public virtual Task DeleteAsync(TEntity entity)
        {
            this.Delete(entity);
            return Task.FromResult(0);
        }

        public abstract void Delete(TPrimaryKey id);
        
        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            this.Delete(id);
            return Task.FromResult(0);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in this.GetAll().Where(predicate).ToList())
            {
                this.Delete(entity);
            }
        }

        public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            this.Delete(predicate);
            return Task.FromResult(0);
        }

        public virtual int Count()
        {
            return this.GetAll().Count();
        }

        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(this.Count());
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return this.GetAll().Where(predicate).Count();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(this.Count(predicate));
        }

        public virtual long LongCount()
        {
            return this.GetAll().LongCount();
        }

        public virtual Task<long> LongCountAsync()
        {
            return Task.FromResult(this.LongCount());
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return this.GetAll().Where(predicate).LongCount();
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(this.LongCount(predicate));
        }

        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}