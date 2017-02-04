using MyCoreFramework.Events.Bus.Handlers;

namespace MyCoreFramework.Events.Bus.Factories.Internals
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory"/> implementation is used to handle events
    /// by a single instance object. 
    /// </summary>
    /// <remarks>
    /// This class always gets the same single instance of handler.
    /// </remarks>
    internal class SingleInstanceHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        /// The event handler instance.
        /// </summary>
        public IEventHandler HandlerInstance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SingleInstanceHandlerFactory(IEventHandler handler)
        {
            this.HandlerInstance = handler;
        }

        public IEventHandler GetHandler()
        {
            return this.HandlerInstance;
        }

        public void ReleaseHandler(IEventHandler handler)
        {
            
        }
    }
}