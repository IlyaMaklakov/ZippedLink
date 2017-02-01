using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace MyCoreFramework.Domain.Uow
{
    internal class UnitOfWorkDefaultOptions : IUnitOfWorkDefaultOptions
    {
        public TransactionScopeOption Scope { get; set; }

        /// <inheritdoc/>
        public bool IsTransactional { get; set; }

        /// <inheritdoc/>
        public TimeSpan? Timeout { get; set; }

        /// <inheritdoc/>
        public IsolationLevel? IsolationLevel { get; set; }

        public IReadOnlyList<DataFilterConfiguration> Filters
        {
            get { return this._filters; }
        }
        private readonly List<DataFilterConfiguration> _filters;

        public void RegisterFilter(string filterName, bool isEnabledByDefault)
        {
            if (this._filters.Any(f => f.FilterName == filterName))
            {
                throw new MyCoreException("There is already a filter with name: " + filterName);
            }

            this._filters.Add(new DataFilterConfiguration(filterName, isEnabledByDefault));
        }

        public void OverrideFilter(string filterName, bool isEnabledByDefault)
        {
            this._filters.RemoveAll(f => f.FilterName == filterName);
            this._filters.Add(new DataFilterConfiguration(filterName, isEnabledByDefault));
        }

        public UnitOfWorkDefaultOptions()
        {
            this._filters = new List<DataFilterConfiguration>();
            this.IsTransactional = true;
            this.Scope = TransactionScopeOption.Required;
        }
    }
}