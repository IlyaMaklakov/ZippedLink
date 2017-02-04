namespace MyCoreFramework.Configuration.Startup
{
    internal class EventBusConfiguration : IEventBusConfiguration
    {
        public bool UseDefaultEventBus { get; set; }

        public EventBusConfiguration()
        {
            this.UseDefaultEventBus = true;
        }
    }
}