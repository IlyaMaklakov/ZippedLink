using System;

namespace MyCoreFramework.Events.Bus.Factories.Internals
{
    /// <summary>
    /// Used to unregister a <see cref="IEventHandlerFactory"/> on <see cref="Dispose"/> method.
    /// </summary>
    internal class FactoryUnregistrar : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly Type _eventType;
        private readonly IEventHandlerFactory _factory;

        public FactoryUnregistrar(IEventBus eventBus, Type eventType, IEventHandlerFactory factory)
        {
            this._eventBus = eventBus;
            this._eventType = eventType;
            this._factory = factory;
        }

        public void Dispose()
        {
            this._eventBus.Unregister(this._eventType, this._factory);
        }
    }
}