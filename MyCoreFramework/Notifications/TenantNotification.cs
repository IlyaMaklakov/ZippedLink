﻿using System;

using MyCoreFramework.Application.Services.Dto;
using MyCoreFramework.Domain.Entities.Auditing;
using MyCoreFramework.Timing;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Represents a published notification for a tenant/user.
    /// </summary>
    [Serializable]
    public class TenantNotification : EntityDto<Guid>, IHasCreationTime
    {
        /// <summary>
        /// Tenant Id.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Unique notification name.
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        /// Notification data.
        /// </summary>
        public NotificationData Data { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Name of the entity type (including namespaces).
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Entity id.
        /// </summary>
        public object EntityId { get; set; }

        /// <summary>
        /// Severity.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantNotification"/> class.
        /// </summary>
        public TenantNotification()
        {
            this.CreationTime = Clock.Now;
        }
    }
}