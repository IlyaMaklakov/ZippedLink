using System.Data.Entity;

using MyCoreFramework.MultiTenancy;

namespace MyCore.EntityFramework
{
    public sealed class SimpleDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public TDbContext DbContext { get; }

        public SimpleDbContextProvider(TDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public TDbContext GetDbContext()
        {
            return this.DbContext;
        }

        public TDbContext GetDbContext(MultiTenancySides? multiTenancySide)
        {
            return this.DbContext;
        }
    }
}