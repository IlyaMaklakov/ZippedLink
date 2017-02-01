using System.Transactions;

using MyCoreFramework.Dependency;

namespace MyCoreFramework.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// </summary>
    internal class UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly ICurrentUnitOfWorkProvider currentUnitOfWorkProvider;
        private readonly IUnitOfWorkDefaultOptions defaultOptions;

        public IActiveUnitOfWork Current => this.currentUnitOfWorkProvider.Current;

        public UnitOfWorkManager(
            IIocResolver iocResolver,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IUnitOfWorkDefaultOptions defaultOptions)
        {
            this.iocResolver = iocResolver;
            this.currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            this.defaultOptions = defaultOptions;
        }

        public IUnitOfWorkCompleteHandle Begin()
        {
            return this.Begin(new UnitOfWorkOptions());
        }

        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
            return this.Begin(new UnitOfWorkOptions { Scope = scope });
        }

        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            options.FillDefaultsForNonProvidedOptions(this.defaultOptions);

            if (options.Scope == TransactionScopeOption.Required && this.currentUnitOfWorkProvider.Current != null)
            {
                return new InnerUnitOfWorkCompleteHandle();
            }

            var uow = this.iocResolver.Resolve<IUnitOfWork>();

            uow.Completed += (sender, args) =>
            {
                this.currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                this.currentUnitOfWorkProvider.Current = null;
            };

            uow.Disposed += (sender, args) =>
            {
                this.iocResolver.Release(uow);
            };

            uow.Begin(options);

            this.currentUnitOfWorkProvider.Current = uow;

            return uow;
        }
    }
}