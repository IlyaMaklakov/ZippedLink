using System.Transactions;

using MyCoreFramework.Dependency;

namespace MyCoreFramework.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// </summary>
    internal class UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly IUnitOfWorkDefaultOptions _defaultOptions;

        public IActiveUnitOfWork Current
        {
            get { return this._currentUnitOfWorkProvider.Current; }
        }

        public UnitOfWorkManager(
            IIocResolver iocResolver,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IUnitOfWorkDefaultOptions defaultOptions)
        {
            this._iocResolver = iocResolver;
            this._currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            this._defaultOptions = defaultOptions;
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
            options.FillDefaultsForNonProvidedOptions(this._defaultOptions);

            if (options.Scope == TransactionScopeOption.Required && this._currentUnitOfWorkProvider.Current != null)
            {
                return new InnerUnitOfWorkCompleteHandle();
            }

            var uow = this._iocResolver.Resolve<IUnitOfWork>();

            uow.Completed += (sender, args) =>
            {
                this._currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                this._currentUnitOfWorkProvider.Current = null;
            };

            uow.Disposed += (sender, args) =>
            {
                this._iocResolver.Release(uow);
            };

            uow.Begin(options);

            this._currentUnitOfWorkProvider.Current = uow;

            return uow;
        }
    }
}