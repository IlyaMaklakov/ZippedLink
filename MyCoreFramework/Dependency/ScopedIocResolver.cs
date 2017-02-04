using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCoreFramework.Dependency
{
    public class ScopedIocResolver : IScopedIocResolver
    {
        private readonly IIocResolver _iocResolver;
        private readonly List<object> _resolvedObjects;

        public ScopedIocResolver(IIocResolver iocResolver)
        {
            this._iocResolver = iocResolver;
            this._resolvedObjects = new List<object>();
        }

        public T Resolve<T>()
        {
            return this.Resolve<T>(typeof(T));
        }

        public T Resolve<T>(Type type)
        {
            return (T)this.Resolve(type);
        }

        public T Resolve<T>(object argumentsAsAnonymousType)
        {
            return (T)this.Resolve(typeof(T), argumentsAsAnonymousType);
        }

        public object Resolve(Type type)
        {
            return this.Resolve(type, null);
        }

        public object Resolve(Type type, object argumentsAsAnonymousType)
        {
            var resolvedObject = argumentsAsAnonymousType != null
                ? this._iocResolver.Resolve(type, argumentsAsAnonymousType)
                : this._iocResolver.Resolve(type);

            this._resolvedObjects.Add(resolvedObject);
            return resolvedObject;
        }

        public T[] ResolveAll<T>()
        {
            return this.ResolveAll(typeof(T)).OfType<T>().ToArray();
        }

        public T[] ResolveAll<T>(object argumentsAsAnonymousType)
        {
            return this.ResolveAll(typeof(T), argumentsAsAnonymousType).OfType<T>().ToArray();
        }

        public object[] ResolveAll(Type type)
        {
            return this.ResolveAll(type, null);
        }

        public object[] ResolveAll(Type type, object argumentsAsAnonymousType)
        {
            var resolvedObjects = argumentsAsAnonymousType != null
                ? this._iocResolver.ResolveAll(type, argumentsAsAnonymousType)
                : this._iocResolver.ResolveAll(type);

            this._resolvedObjects.AddRange(resolvedObjects);
            return resolvedObjects;
        }

        public void Release(object obj)
        {
            this._resolvedObjects.Remove(obj);
            this._iocResolver.Release(obj);
        }

        public bool IsRegistered(Type type)
        {
            return this._iocResolver.IsRegistered(type);
        }

        public bool IsRegistered<T>()
        {
            return this.IsRegistered(typeof(T));
        }

        public void Dispose()
        {
            this._resolvedObjects.ForEach(this._iocResolver.Release);
        }
    }
}
