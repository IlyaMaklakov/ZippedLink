using System.Collections.Generic;
using System.Data.Entity;

namespace MyCore.EntityFramework.Uow
{
    public class ActiveTransactionInfo
    {
        public DbContextTransaction DbContextTransaction { get; }

        public DbContext StarterDbContext { get; }

        public List<DbContext> AttendedDbContexts { get; }

        public ActiveTransactionInfo(DbContextTransaction dbContextTransaction, DbContext starterDbContext)
        {
            this.DbContextTransaction = dbContextTransaction;
            this.StarterDbContext = starterDbContext;

            this.AttendedDbContexts = new List<DbContext>();
        }
    }
}