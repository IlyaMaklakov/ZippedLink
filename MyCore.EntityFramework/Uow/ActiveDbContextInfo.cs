using System.Data.Entity;

namespace MyCore.EntityFramework.Uow
{
    public class ActiveDbContextInfo
    {
        public DbContext DbContext { get; }

        public ActiveDbContextInfo(DbContext dbContext)
        {
            this.DbContext = dbContext;
        }
    }
}