using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Castle.Core;

using MyCoreFramework.Extensions;
using MyCoreFramework.Runtime.Session;

namespace MyCoreFramework.Domain.Uow
{
    /// <summary>
    /// Base for all Unit Of Work classes.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public string Id { get; }

        [DoNotWire]
        public IUnitOfWork Outer { get; set; }

        /// <inheritdoc/>
        public event EventHandler Completed;

        /// <inheritdoc/>
        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <inheritdoc/>
        public event EventHandler Disposed;

        /// <inheritdoc/>
        public UnitOfWorkOptions Options { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<DataFilterConfiguration> Filters
        {
            get { return this._filters.ToImmutableList(); }
        }
        private readonly List<DataFilterConfiguration> _filters;

        /// <summary>
        /// Gets default UOW options.
        /// </summary>
        protected IUnitOfWorkDefaultOptions DefaultOptions { get; }

        /// <summary>
        /// Gets the connection string resolver.
        /// </summary>
        protected IConnectionStringResolver ConnectionStringResolver { get; }

        /// <summary>
        /// Gets a value indicates that this unit of work is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Reference to current ABP session.
        /// </summary>
        public IAbpSession AbpSession { protected get; set; }

        protected IUnitOfWorkFilterExecuter FilterExecuter { get; }

        /// <summary>
        /// Is <see cref="Begin"/> method called before?
        /// </summary>
        private bool _isBeginCalledBefore;

        /// <summary>
        /// Is <see cref="Complete"/> method called before?
        /// </summary>
        private bool _isCompleteCalledBefore;

        /// <summary>
        /// Is this unit of work successfully completed.
        /// </summary>
        private bool _succeed;

        /// <summary>
        /// A reference to the exception if this unit of work failed.
        /// </summary>
        private Exception _exception;

        private int? _tenantId;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected UnitOfWorkBase(
            IConnectionStringResolver connectionStringResolver, 
            IUnitOfWorkDefaultOptions defaultOptions,
            IUnitOfWorkFilterExecuter filterExecuter)
        {
            this.FilterExecuter = filterExecuter;
            this.DefaultOptions = defaultOptions;
            this.ConnectionStringResolver = connectionStringResolver;

            this.Id = Guid.NewGuid().ToString("N");
            this._filters = defaultOptions.Filters.ToList();

            this.AbpSession = NullAbpSession.Instance;
        }

        /// <inheritdoc/>
        public void Begin(UnitOfWorkOptions options)
        {
            Check.NotNull(options, nameof(options));

            this.PreventMultipleBegin();
            this.Options = options; //TODO: Do not set options like that, instead make a copy?

            this.SetFilters(options.FilterOverrides);

            this.SetTenantId(this.AbpSession.TenantId);

            this.BeginUow();
        }

        /// <inheritdoc/>
        public abstract void SaveChanges();

        /// <inheritdoc/>
        public abstract Task SaveChangesAsync();

        /// <inheritdoc/>
        public IDisposable DisableFilter(params string[] filterNames)
        {
            //TODO: Check if filters exists?

            var disabledFilters = new List<string>();

            foreach (var filterName in filterNames)
            {
                var filterIndex = this.GetFilterIndex(filterName);
                if (this._filters[filterIndex].IsEnabled)
                {
                    disabledFilters.Add(filterName);
                    this._filters[filterIndex] = new DataFilterConfiguration(this._filters[filterIndex], false);
                }
            }

            disabledFilters.ForEach(this.ApplyDisableFilter);

            return new DisposeAction(() => this.EnableFilter(disabledFilters.ToArray()));
        }

        /// <inheritdoc/>
        public IDisposable EnableFilter(params string[] filterNames)
        {
            //TODO: Check if filters exists?

            var enabledFilters = new List<string>();

            foreach (var filterName in filterNames)
            {
                var filterIndex = this.GetFilterIndex(filterName);
                if (!this._filters[filterIndex].IsEnabled)
                {
                    enabledFilters.Add(filterName);
                    this._filters[filterIndex] = new DataFilterConfiguration(this._filters[filterIndex], true);
                }
            }

            enabledFilters.ForEach(this.ApplyEnableFilter);

            return new DisposeAction(() => this.DisableFilter(enabledFilters.ToArray()));
        }

        /// <inheritdoc/>
        public bool IsFilterEnabled(string filterName)
        {
            return this.GetFilter(filterName).IsEnabled;
        }

        /// <inheritdoc/>
        public IDisposable SetFilterParameter(string filterName, string parameterName, object value)
        {
            var filterIndex = this.GetFilterIndex(filterName);

            var newfilter = new DataFilterConfiguration(this._filters[filterIndex]);

            //Store old value
            object oldValue = null;
            var hasOldValue = newfilter.FilterParameters.ContainsKey(parameterName);
            if (hasOldValue)
            {
                oldValue = newfilter.FilterParameters[parameterName];
            }

            newfilter.FilterParameters[parameterName] = value;

            this._filters[filterIndex] = newfilter;

            this.ApplyFilterParameterValue(filterName, parameterName, value);

            return new DisposeAction(() =>
            {
                //Restore old value
                if (hasOldValue)
                {
                    this.SetFilterParameter(filterName, parameterName, oldValue);
                }
            });
        }

        public IDisposable SetTenantId(int? tenantId)
        {
            var oldTenantId = this._tenantId;
            this._tenantId = tenantId;

            var mustHaveTenantEnableChange = tenantId == null
                ? this.DisableFilter(AbpDataFilters.MustHaveTenant)
                : this.EnableFilter(AbpDataFilters.MustHaveTenant);

            var mayHaveTenantChange = this.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, tenantId);
            var mustHaveTenantChange = this.SetFilterParameter(AbpDataFilters.MustHaveTenant, AbpDataFilters.Parameters.TenantId, tenantId ?? 0);

            return new DisposeAction(() =>
            {
                mayHaveTenantChange.Dispose();
                mustHaveTenantChange.Dispose();
                mustHaveTenantEnableChange.Dispose();
                this._tenantId = oldTenantId;
            });
        }

        public int? GetTenantId()
        {
            return this._tenantId;
        }

        /// <inheritdoc/>
        public void Complete()
        {
            this.PreventMultipleComplete();
            try
            {
                this.CompleteUow();
                this._succeed = true;
                this.OnCompleted();
            }
            catch (Exception ex)
            {
                this._exception = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            this.PreventMultipleComplete();
            try
            {
                await this.CompleteUowAsync();
                this._succeed = true;
                this.OnCompleted();
            }
            catch (Exception ex)
            {
                this._exception = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this._isBeginCalledBefore || this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;

            if (!this._succeed)
            {
                this.OnFailed(this._exception);
            }

            this.DisposeUow();
            this.OnDisposed();
        }

        /// <summary>
        /// Can be implemented by derived classes to start UOW.
        /// </summary>
        protected virtual void BeginUow()
        {
            
        }

        /// <summary>
        /// Should be implemented by derived classes to complete UOW.
        /// </summary>
        protected abstract void CompleteUow();

        /// <summary>
        /// Should be implemented by derived classes to complete UOW.
        /// </summary>
        protected abstract Task CompleteUowAsync();

        /// <summary>
        /// Should be implemented by derived classes to dispose UOW.
        /// </summary>
        protected abstract void DisposeUow();

        protected virtual void ApplyDisableFilter(string filterName)
        {
            this.FilterExecuter.ApplyDisableFilter(this, filterName);
        }

        protected virtual void ApplyEnableFilter(string filterName)
        {
            this.FilterExecuter.ApplyEnableFilter(this, filterName);
        }

        protected virtual void ApplyFilterParameterValue(string filterName, string parameterName, object value)
        {
            this.FilterExecuter.ApplyFilterParameterValue(this, filterName, parameterName, value);
        }

        protected virtual string ResolveConnectionString(ConnectionStringResolveArgs args)
        {
            return this.ConnectionStringResolver.GetNameOrConnectionString(args);
        }

        /// <summary>
        /// Called to trigger <see cref="Completed"/> event.
        /// </summary>
        protected virtual void OnCompleted()
        {
            this.Completed.InvokeSafely(this);
        }

        /// <summary>
        /// Called to trigger <see cref="Failed"/> event.
        /// </summary>
        /// <param name="exception">Exception that cause failure</param>
        protected virtual void OnFailed(Exception exception)
        {
            this.Failed.InvokeSafely(this, new UnitOfWorkFailedEventArgs(exception));
        }

        /// <summary>
        /// Called to trigger <see cref="Disposed"/> event.
        /// </summary>
        protected virtual void OnDisposed()
        {
            this.Disposed.InvokeSafely(this);
        }

        private void PreventMultipleBegin()
        {
            if (this._isBeginCalledBefore)
            {
                throw new AbpException("This unit of work has started before. Can not call Start method more than once.");
            }

            this._isBeginCalledBefore = true;
        }

        private void PreventMultipleComplete()
        {
            if (this._isCompleteCalledBefore)
            {
                throw new AbpException("Complete is called before!");
            }

            this._isCompleteCalledBefore = true;
        }

        private void SetFilters(List<DataFilterConfiguration> filterOverrides)
        {
            for (var i = 0; i < this._filters.Count; i++)
            {
                var filterOverride = filterOverrides.FirstOrDefault(f => f.FilterName == this._filters[i].FilterName);
                if (filterOverride != null)
                {
                    this._filters[i] = filterOverride;
                }
            }

            if (this.AbpSession.TenantId == null)
            {
                this.ChangeFilterIsEnabledIfNotOverrided(filterOverrides, AbpDataFilters.MustHaveTenant, false);
            }
        }

        private void ChangeFilterIsEnabledIfNotOverrided(List<DataFilterConfiguration> filterOverrides, string filterName, bool isEnabled)
        {
            if (filterOverrides.Any(f => f.FilterName == filterName))
            {
                return;
            }

            var index = this._filters.FindIndex(f => f.FilterName == filterName);
            if (index < 0)
            {
                return;
            }

            if (this._filters[index].IsEnabled == isEnabled)
            {
                return;
            }

            this._filters[index] = new DataFilterConfiguration(filterName, isEnabled);
        }

        private DataFilterConfiguration GetFilter(string filterName)
        {
            var filter = this._filters.FirstOrDefault(f => f.FilterName == filterName);
            if (filter == null)
            {
                throw new AbpException("Unknown filter name: " + filterName + ". Be sure this filter is registered before.");
            }

            return filter;
        }

        private int GetFilterIndex(string filterName)
        {
            var filterIndex = this._filters.FindIndex(f => f.FilterName == filterName);
            if (filterIndex < 0)
            {
                throw new AbpException("Unknown filter name: " + filterName + ". Be sure this filter is registered before.");
            }

            return filterIndex;
        }
    }
}