﻿using System;
using System.Linq;
using System.Threading.Tasks;

using MyCoreFramework.BackgroundJobs;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Entities;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Extensions;
using MyCoreFramework.Json;
using MyCoreFramework.Runtime.Session;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationPublisher"/>.
    /// </summary>
    public class NotificationPublisher : AbpServiceBase, INotificationPublisher, ITransientDependency
    {
        public const int MaxUserCountToDirectlyDistributeANotification = 5;

        /// <summary>
        /// Indicates all tenants.
        /// </summary>
        public static int[] AllTenants
        {
            get
            {
                return new[] { NotificationInfo.AllTenantIds.To<int>() };
            }
        }

        /// <summary>
        /// Reference to ABP session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        private readonly INotificationStore _store;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly INotificationDistributer _notificationDistributer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPublisher"/> class.
        /// </summary>
        public NotificationPublisher(
            INotificationStore store,
            IBackgroundJobManager backgroundJobManager,
            INotificationDistributer notificationDistributer)
        {
            this._store = store;
            this._backgroundJobManager = backgroundJobManager;
            this._notificationDistributer = notificationDistributer;
            this.AbpSession = NullAbpSession.Instance;
        }

        //Create EntityIdentifier includes entityType and entityId.
        [UnitOfWork]
        public virtual async Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            int?[] tenantIds = null)
        {
            if (notificationName.IsNullOrEmpty())
            {
                throw new ArgumentException("NotificationName can not be null or whitespace!", "notificationName");
            }

            if (!tenantIds.IsNullOrEmpty() && !userIds.IsNullOrEmpty())
            {
                throw new ArgumentException("tenantIds can be set only if userIds is not set!", "tenantIds");
            }

            if (tenantIds.IsNullOrEmpty() && userIds.IsNullOrEmpty())
            {
                tenantIds = new[] {this.AbpSession.TenantId};
            }

            var notificationInfo = new NotificationInfo
            {
                NotificationName = notificationName,
                EntityTypeName = entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                EntityTypeAssemblyQualifiedName = entityIdentifier == null ? null : entityIdentifier.Type.AssemblyQualifiedName,
                EntityId = entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString(),
                Severity = severity,
                UserIds = userIds.IsNullOrEmpty() ? null : userIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                ExcludedUserIds = excludedUserIds.IsNullOrEmpty() ? null : excludedUserIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
                TenantIds = tenantIds.IsNullOrEmpty() ? null : tenantIds.JoinAsString(","),
                Data = data == null ? null : data.ToJsonString(),
                DataTypeName = data == null ? null : data.GetType().AssemblyQualifiedName
            };

            await this._store.InsertNotificationAsync(notificationInfo);

            await this.CurrentUnitOfWork.SaveChangesAsync(); //To get Id of the notification

            if (userIds != null && userIds.Length <= MaxUserCountToDirectlyDistributeANotification)
            {
                //We can directly distribute the notification since there are not much receivers
                await this._notificationDistributer.DistributeAsync(notificationInfo.Id);
            }
            else
            {
                //We enqueue a background job since distributing may get a long time
                await this._backgroundJobManager.EnqueueAsync<NotificationDistributionJob, NotificationDistributionJobArgs>(
                    new NotificationDistributionJobArgs(
                        notificationInfo.Id
                        )
                    );
            }
        }
    }
}