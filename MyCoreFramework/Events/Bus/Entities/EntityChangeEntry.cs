namespace MyCoreFramework.Events.Bus.Entities
{
    public class EntityChangeEntry
    {
        public object Entity { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public EntityChangeEntry(object entity, EntityChangeType changeType)
        {
            this.Entity = entity;
            this.ChangeType = changeType;
        }
    }
}