using System;

using MyCoreFramework.Dependency;
using MyCoreFramework.Events.Bus.Handlers;

namespace MyCoreFramework.Events.Bus.Factories
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory"/> implementation is used to get/release
    /// handlers using Ioc.
    /// </summary>
    public class IocHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        /// Type of the handler.
        /// </summary>
        public Type HandlerType { get; private set; }

        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Creates a new instance of <see cref="IocHandlerFactory"/> class.
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <param name="handlerType">Type of the handler</param>
        public IocHandlerFactory(IIocResolver iocResolver, Type handlerType)
        {
            this._iocResolver = iocResolver;
            this.HandlerType = handlerType;
        }

        /// <summary>
        /// Resolves handler object from Ioc container.
        /// </summary>
        /// <returns>Resolved handler object</returns>
        public IEventHandler GetHandler()
        {
            return (IEventHandler)this._iocResolver.Resolve(this.HandlerType);
        }

        /// <summary>
        /// Releases handler object using Ioc container.
        /// </summary>
        /// <param name="handler">Handler to be released</param>
        public void ReleaseHandler(IEventHandler handler)
        {
            this._iocResolver.Release(handler);
        }
    }
}