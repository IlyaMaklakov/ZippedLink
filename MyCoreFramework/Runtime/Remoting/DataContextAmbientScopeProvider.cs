using System;
using System.Collections.Concurrent;

using Castle.Core.Logging;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.JetBrains.Annotations;

namespace MyCoreFramework.Runtime.Remoting
{
    /// <summary>
    /// CallContext implementation of <see cref="ICurrentUnitOfWorkProvider"/>. 
    /// This is the default implementation.
    /// </summary>
    public class DataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
    {
        public ILogger Logger { get; set; }

        private static readonly ConcurrentDictionary<string, ScopeItem> ScopeDictionary = new ConcurrentDictionary<string, ScopeItem>();

        private readonly IAmbientDataContext _dataContext;

        public DataContextAmbientScopeProvider([NotNull] IAmbientDataContext dataContext)
        {
            Check.NotNull(dataContext, nameof(dataContext));

            this._dataContext = dataContext;

            this.Logger = NullLogger.Instance;
        }

        public T GetValue(string contextKey)
        {
            var item = this.GetCurrentItem(contextKey);
            if (item == null)
            {
                return default(T);
            }
            return item.Value;
        }

        public IDisposable BeginScope(string contextKey, T value)
        {
            var item = new ScopeItem(value, this.GetCurrentItem(contextKey));

            if (!ScopeDictionary.TryAdd(item.Id, item))
            {
                throw new AbpException("Can not set unit of work! ScopeDictionary.TryAdd returns false!");
            }

            this._dataContext.SetData(contextKey, item.Id);

            return new DisposeAction(() =>
            {
                ScopeDictionary.TryRemove(item.Id, out item);

                if (item.Outer == null)
                {
                    this._dataContext.SetData(contextKey, null);
                    return;
                }

                this._dataContext.SetData(contextKey, item.Outer.Id);
            });
        }

        private ScopeItem GetCurrentItem(string contextKey)
        {
            var objKey = this._dataContext.GetData(contextKey) as string;
            return objKey != null ? ScopeDictionary.GetOrDefault(objKey) : null;
        }

        private class ScopeItem
        {
            public string Id { get; }

            public ScopeItem Outer { get; }

            public T Value { get; }

            public ScopeItem(T value, ScopeItem outer = null)
            {
                this.Id = Guid.NewGuid().ToString();

                this.Value = value;
                this.Outer = outer;
            }
        }
    }
}