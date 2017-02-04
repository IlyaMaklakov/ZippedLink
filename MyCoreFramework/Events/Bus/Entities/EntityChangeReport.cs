using System.Collections.Generic;

namespace MyCoreFramework.Events.Bus.Entities
{
    public class EntityChangeReport
    {
        public List<EntityChangeEntry> ChangedEntities { get; }

        public List<DomainEventEntry> DomainEvents { get; }

        public EntityChangeReport()
        {
            this.ChangedEntities = new List<EntityChangeEntry>();
            this.DomainEvents = new List<DomainEventEntry>();
        }

        public bool IsEmpty()
        {
            return this.ChangedEntities.Count <= 0 && this.DomainEvents.Count <= 0;
        }

        public override string ToString()
        {
            return $"[EntityChangeReport] ChangedEntities: {this.ChangedEntities.Count}, DomainEvents: {this.DomainEvents.Count}";
        }
    }
}