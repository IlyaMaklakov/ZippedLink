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

        private readonly IAmbientDataContext dataContext;

        public DataContextAmbientScopeProvider([NotNull] IAmbientDataContext dataContext)
        {
            Check.NotNull(dataContext, nameof(dataContext));

            this.dataContext = dataContext;

            Logger = NullLogger.Instance;
        }

        public T GetValue(string contextKey)
        {
            var item = GetCurrentItem(contextKey);
            if (item == null)
            {
                return default(T);
            }
            return item.Value;
        }

        public IDisposable BeginScope(string contextKey, T value)
        {
            var item = new ScopeItem(value, GetCurrentItem(contextKey));

            if (!ScopeDictionary.TryAdd(item.Id, item))
            {
                throw new MyCoreException("Can not set unit of work! ScopeDictionary.TryAdd returns false!");
            }

            this.dataContext.SetData(contextKey, item.Id);

            return new DisposeAction(() =>
            {
                ScopeDictionary.TryRemove(item.Id, out item);

                if (item.Outer == null)
                {
                    this.dataContext.SetData(contextKey, null);
                    return;
                }

                this.dataContext.SetData(contextKey, item.Outer.Id);
            });
        }

        private ScopeItem GetCurrentItem(string contextKey)
        {
            var objKey = this.dataContext.GetData(contextKey) as string;
            return objKey != null ? ScopeDictionary.GetOrDefault(objKey) : null;
        }

        private class ScopeItem
        {
            public string Id { get; }

            public ScopeItem Outer { get; }

            public T Value { get; }

            public ScopeItem(T value, ScopeItem outer = null)
            {
                Id = Guid.NewGuid().ToString();

                Value = value;
                Outer = outer;
            }
        }
    }
}
