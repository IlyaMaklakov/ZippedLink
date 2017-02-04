using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

using MyCoreFramework.Events.Bus;

namespace MyCoreFramework.Domain.Entities
{
    public class AggregateRoot : AggregateRoot<int>, IAggregateRoot
    {

    }

    public class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {
        [NotMapped]
        public virtual ICollection<IEventData> DomainEvents { get; }

        public AggregateRoot()
        {
            this.DomainEvents = new Collection<IEventData>();
        }
    }
}