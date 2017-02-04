using MyCore.EntityFramework;
using MyCore.EntityFramework.Repositories;

using MyCoreFramework.Domain.Entities;

namespace ZippedLink.EntityFramework.EntityFramework.Repositories
{
    /// <summary>
    /// We declare a base repository class for our application.
    /// It inherits from <see cref="EfRepositoryBase{TDbContext,TEntity,TPrimaryKey}"/>.
    /// We can add here common methods for all our repositories.
    /// </summary>
    public abstract class ZippedLinkRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<ZippedLinkDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ZippedLinkRepositoryBase(IDbContextProvider<ZippedLinkDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    /// <summary>
    /// A shortcut of <see cref="SimpleTaskSystemRepositoryBase{TEntity,TPrimaryKey}"/> for Entities with primary key type <see cref="int"/>.
    /// </summary>
    public abstract class ZippedLinkRepositoryBase<TEntity> : ZippedLinkRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ZippedLinkRepositoryBase(IDbContextProvider<ZippedLinkDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
