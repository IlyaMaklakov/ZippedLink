using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

using Castle.Core.Internal;

using MyCore.EntityFramework.Utils;

using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Extensions;
using MyCoreFramework.MultiTenancy;

namespace MyCore.EntityFramework.Uow
{
    /// <summary>
    /// Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        protected IDictionary<string, DbContext> ActiveDbContexts { get; }
        protected IIocResolver IocResolver { get; }

        private readonly IDbContextResolver _dbContextResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;
        private readonly IEfTransactionStrategy _transactionStrategy;

        /// <summary>
        /// Creates a new <see cref="EfUnitOfWork"/>.
        /// </summary>
        public EfUnitOfWork(
            IIocResolver iocResolver,
            IConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver,
            IEfUnitOfWorkFilterExecuter filterExecuter,
            IUnitOfWorkDefaultOptions defaultOptions,
            IDbContextTypeMatcher dbContextTypeMatcher,
            IEfTransactionStrategy transactionStrategy)
            : base(
                  connectionStringResolver,
                  defaultOptions,
                  filterExecuter)
        {
            this.IocResolver = iocResolver;
            this._dbContextResolver = dbContextResolver;
            this._dbContextTypeMatcher = dbContextTypeMatcher;
            this._transactionStrategy = transactionStrategy;

            this.ActiveDbContexts = new Dictionary<string, DbContext>();
        }

        protected override void BeginUow()
        {
            if (this.Options.IsTransactional == true)
            {
                this._transactionStrategy.InitOptions(this.Options);
            }
        }

        public override void SaveChanges()
        {
            this.ActiveDbContexts.Values.ForEach(this.SaveChangesInDbContext);
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in this.ActiveDbContexts.Values)
            {
                await this.SaveChangesInDbContextAsync(dbContext);
            }
        }

        public IReadOnlyList<DbContext> GetAllActiveDbContexts()
        {
            return this.ActiveDbContexts.Values.ToImmutableList();
        }

        protected override void CompleteUow()
        {
            this.SaveChanges();

            if (this.Options.IsTransactional == true)
            {
                this._transactionStrategy.Commit();
            }
        }

        protected override async Task CompleteUowAsync()
        {
            await this.SaveChangesAsync();

            if (this.Options.IsTransactional == true)
            {
                this._transactionStrategy.Commit();
            }
        }
        
        public virtual TDbContext GetOrCreateDbContext<TDbContext>(MultiTenancySides? multiTenancySide = null)
            where TDbContext : DbContext
        {
            var concreteDbContextType = this._dbContextTypeMatcher.GetConcreteType(typeof(TDbContext));

            var connectionStringResolveArgs = new ConnectionStringResolveArgs(multiTenancySide);
            connectionStringResolveArgs["DbContextType"] = typeof(TDbContext);
            connectionStringResolveArgs["DbContextConcreteType"] = concreteDbContextType;
            var connectionString = this.ResolveConnectionString(connectionStringResolveArgs);

            var dbContextKey = concreteDbContextType.FullName + "#" + connectionString;

            DbContext dbContext;
            if (!this.ActiveDbContexts.TryGetValue(dbContextKey, out dbContext))
            {
                if (this.Options.IsTransactional == true)
                {
                    dbContext = this._transactionStrategy.CreateDbContext<TDbContext>(connectionString, this._dbContextResolver);
                }
                else
                {
                    dbContext = this._dbContextResolver.Resolve<TDbContext>(connectionString);
                }

                ((IObjectContextAdapter)dbContext).ObjectContext.ObjectMaterialized += (sender, args) =>
                {
                    ObjectContext_ObjectMaterialized(dbContext, args);
                };

                this.FilterExecuter.As<IEfUnitOfWorkFilterExecuter>().ApplyCurrentFilters(this, dbContext);
                
                this.ActiveDbContexts[dbContextKey] = dbContext;
            }

            return (TDbContext)dbContext;
        }

        protected override void DisposeUow()
        {
            if (this.Options.IsTransactional == true)
            {
                this._transactionStrategy.Dispose(this.IocResolver);
            }
            else
            {
                foreach (var activeDbContext in this.ActiveDbContexts.Values)
                {
                    this.Release(activeDbContext);
                }
            }

            this.ActiveDbContexts.Clear();
        }

        protected virtual void SaveChangesInDbContext(DbContext dbContext)
        {
            dbContext.SaveChanges();
        }

        protected virtual async Task SaveChangesInDbContextAsync(DbContext dbContext)
        {
            await dbContext.SaveChangesAsync();
        }

        protected virtual void Release(DbContext dbContext)
        {
            dbContext.Dispose();
            this.IocResolver.Release(dbContext);
        }

        private static void ObjectContext_ObjectMaterialized(DbContext dbContext, ObjectMaterializedEventArgs e)
        {
            var entityType = ObjectContext.GetObjectType(e.Entity.GetType());

            dbContext.Configuration.AutoDetectChangesEnabled = false;
            var previousState = dbContext.Entry(e.Entity).State;

            DateTimePropertyInfoHelper.NormalizeDatePropertyKinds(e.Entity, entityType);

            dbContext.Entry(e.Entity).State = previousState;
            dbContext.Configuration.AutoDetectChangesEnabled = true;
        }
    }
}