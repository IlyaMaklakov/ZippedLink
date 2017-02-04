namespace MyCoreFramework.Events.Bus.Entities
{
    public class DomainEventEntry
    {
        public object SourceEntity { get; }

        public IEventData EventData { get; }

        public DomainEventEntry(object sourceEntity, IEventData eventData)
        {
            this.SourceEntity = sourceEntity;
            this.EventData = eventData;
        }
    }
}