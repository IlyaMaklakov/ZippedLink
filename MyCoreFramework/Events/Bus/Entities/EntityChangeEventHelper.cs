using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;

namespace MyCoreFramework.Events.Bus.Entities
{
    /// <summary>
    /// Used to trigger entity change events.
    /// </summary>
    public class EntityChangeEventHelper : ITransientDependency, IEntityChangeEventHelper
    {
        public IEventBus EventBus { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EntityChangeEventHelper(IUnitOfWorkManager unitOfWorkManager)
        {
            this._unitOfWorkManager = unitOfWorkManager;
            this.EventBus = NullEventBus.Instance;
        }

        public virtual void TriggerEvents(EntityChangeReport changeReport)
        {
            this.TriggerEventsInternal(changeReport);

            if (changeReport.IsEmpty() || this._unitOfWorkManager.Current == null)
            {
                return;
            }

            this._unitOfWorkManager.Current.SaveChanges();
        }

        public Task TriggerEventsAsync(EntityChangeReport changeReport)
        {
            this.TriggerEventsInternal(changeReport);

            if (changeReport.IsEmpty() || this._unitOfWorkManager.Current == null)
            {
                return Task.FromResult(0);
            }

            return this._unitOfWorkManager.Current.SaveChangesAsync();
        }

        public virtual void TriggerEntityCreatingEvent(object entity)
        {
            this.TriggerEventWithEntity(typeof(EntityCreatingEventData<>), entity, true);
        }

        public virtual void TriggerEntityCreatedEventOnUowCompleted(object entity)
        {
            this.TriggerEventWithEntity(typeof(EntityCreatedEventData<>), entity, false);
        }

        public virtual void TriggerEntityUpdatingEvent(object entity)
        {
            this.TriggerEventWithEntity(typeof(EntityUpdatingEventData<>), entity, true);
        }

        public virtual void TriggerEntityUpdatedEventOnUowCompleted(object entity)
        {
            this.TriggerEventWithEntity(typeof(EntityUpdatedEventData<>), entity, false);
        }

        public virtual void TriggerEntityDeletingEvent(object entity)
        {
            this.TriggerEventWithEntity(typeof(EntityDeletingEventData<>), entity, true);
        }

        public virtual void TriggerEntityDeletedEventOnUowCompleted(object entity)
        {
            this.TriggerEventWithEntity(typeof(EntityDeletedEventData<>), entity, false);
        }

        public virtual void TriggerEventsInternal(EntityChangeReport changeReport)
        {
            this.TriggerEntityChangeEvents(changeReport.ChangedEntities);
            this.TriggerDomainEvents(changeReport.DomainEvents);
        }

        protected virtual void TriggerEntityChangeEvents(List<EntityChangeEntry> changedEntities)
        {
            foreach (var changedEntity in changedEntities)
            {
                switch (changedEntity.ChangeType)
                {
                    case EntityChangeType.Created:
                        this.TriggerEntityCreatingEvent(changedEntity.Entity);
                        this.TriggerEntityCreatedEventOnUowCompleted(changedEntity.Entity);
                        break;
                    case EntityChangeType.Updated:
                        this.TriggerEntityUpdatingEvent(changedEntity.Entity);
                        this.TriggerEntityUpdatedEventOnUowCompleted(changedEntity.Entity);
                        break;
                    case EntityChangeType.Deleted:
                        this.TriggerEntityDeletingEvent(changedEntity.Entity);
                        this.TriggerEntityDeletedEventOnUowCompleted(changedEntity.Entity);
                        break;
                    default:
                        throw new AbpException("Unknown EntityChangeType: " + changedEntity.ChangeType);
                }
            }
        }

        protected virtual void TriggerDomainEvents(List<DomainEventEntry> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                this.EventBus.Trigger(domainEvent.EventData.GetType(), domainEvent.SourceEntity, domainEvent.EventData);
            }
        }

        protected virtual void TriggerEventWithEntity(Type genericEventType, object entity, bool triggerInCurrentUnitOfWork)
        {
            var entityType = entity.GetType();
            var eventType = genericEventType.MakeGenericType(entityType);

            if (triggerInCurrentUnitOfWork || this._unitOfWorkManager.Current == null)
            {
                this.EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
                return;
            }

            this._unitOfWorkManager.Current.Completed += (sender, args) => this.EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
        }
    }
}