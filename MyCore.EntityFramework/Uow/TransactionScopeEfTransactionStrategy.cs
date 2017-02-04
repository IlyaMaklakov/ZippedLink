using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;

using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;

namespace MyCore.EntityFramework.Uow
{
    public class TransactionScopeEfTransactionStrategy : IEfTransactionStrategy, ITransientDependency
    {
        protected UnitOfWorkOptions Options { get; private set; }

        protected TransactionScope CurrentTransaction { get; set; }

        protected List<DbContext> DbContexts { get; }

        public TransactionScopeEfTransactionStrategy()
        {
            this.DbContexts = new List<DbContext>();
        }

        public virtual void InitOptions(UnitOfWorkOptions options)
        {
            this.Options = options;

            this.StartTransaction();
        }

        public virtual void Commit()
        {
            if (this.CurrentTransaction == null)
            {
                return;
            }

            this.CurrentTransaction.Complete();

            this.CurrentTransaction.Dispose();
            this.CurrentTransaction = null;
        }

        public DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
            where TDbContext : DbContext
        {
            var dbContext = dbContextResolver.Resolve<TDbContext>(connectionString);
            this.DbContexts.Add(dbContext);
            return dbContext;
        }

        private void StartTransaction()
        {
            if (this.CurrentTransaction != null)
            {
                return;
            }

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = this.Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted),
            };

            if (this.Options.Timeout.HasValue)
            {
                transactionOptions.Timeout = this.Options.Timeout.Value;
            }

            this.CurrentTransaction = new TransactionScope(
                this.Options.Scope.GetValueOrDefault(TransactionScopeOption.Required),
                transactionOptions,
                this.Options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled)
            );
        }

        public virtual void Dispose(IIocResolver iocResolver)
        {
            foreach (var dbContext in this.DbContexts)
            {
                iocResolver.Release(dbContext);
            }

            this.DbContexts.Clear();

            if (this.CurrentTransaction != null)
            {
                this.CurrentTransaction.Dispose();
                this.CurrentTransaction = null;
            }
        }
    }
}