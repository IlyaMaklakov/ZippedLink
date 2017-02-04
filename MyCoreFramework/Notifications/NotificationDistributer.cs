using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Castle.Core.Internal;

using MyCoreFramework.Configuration;
using MyCoreFramework.Domain.Services;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Extensions;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Used to distribute notifications to users.
    /// </summary>
    public class NotificationDistributer : DomainService, INotificationDistributer
    {
        public IRealTimeNotifier RealTimeNotifier { get; set; }

        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly INotificationStore _notificationStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
        public NotificationDistributer(
            INotificationDefinitionManager notificationDefinitionManager,
            INotificationStore notificationStore,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this._notificationDefinitionManager = notificationDefinitionManager;
            this._notificationStore = notificationStore;
            this._unitOfWorkManager = unitOfWorkManager;

            this.RealTimeNotifier = NullRealTimeNotifier.Instance;
        }

        public async Task DistributeAsync(Guid notificationId)
        {
            var notificationInfo = await this._notificationStore.GetNotificationOrNullAsync(notificationId);
            if (notificationInfo == null)
            {
                this.Logger.Warn("NotificationDistributionJob can not continue since could not found notification by id: " + notificationId);
                return;
            }

            var users = await this.GetUsers(notificationInfo);

            var userNotifications = await this.SaveUserNotifications(users, notificationInfo);

            await this._notificationStore.DeleteNotificationAsync(notificationInfo);

            try
            {
                await this.RealTimeNotifier.SendNotificationsAsync(userNotifications.ToArray());
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex.ToString(), ex);
            }
        }

        [UnitOfWork]
        protected virtual async Task<UserIdentifier[]> GetUsers(NotificationInfo notificationInfo)
        {
            List<UserIdentifier> userIds;

            if (!notificationInfo.UserIds.IsNullOrEmpty())
            {
                //Directly get from UserIds
                userIds = notificationInfo
                    .UserIds
                    .Split(",")
                    .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                    .Where(uid => this.SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications, uid.TenantId, uid.UserId))
                    .ToList();
            }
            else
            {
                //Get subscribed users

                var tenantIds = GetTenantIds(notificationInfo);

                List<NotificationSubscriptionInfo> subscriptions;

                if (tenantIds.IsNullOrEmpty() ||
                    (tenantIds.Length == 1 && tenantIds[0] == NotificationInfo.AllTenantIds.To<int>()))
                {
                    //Get all subscribed users of all tenants
                    subscriptions = await this._notificationStore.GetSubscriptionsAsync(
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                        );
                }
                else
                {
                    //Get all subscribed users of specified tenant(s)
                    subscriptions = await this._notificationStore.GetSubscriptionsAsync(
                        tenantIds,
                        notificationInfo.NotificationName,
                        notificationInfo.EntityTypeName,
                        notificationInfo.EntityId
                        );
                }

                //Remove invalid subscriptions
                var invalidSubscriptions = new Dictionary<Guid, NotificationSubscriptionInfo>();

                //TODO: Group subscriptions per tenant for potential performance improvement
                foreach (var subscription in subscriptions)
                {
                    using (this.CurrentUnitOfWork.SetTenantId(subscription.TenantId))
                    {
                        if (!await this._notificationDefinitionManager.IsAvailableAsync(notificationInfo.NotificationName, new UserIdentifier(subscription.TenantId, subscription.UserId)) ||
                            !this.SettingManager.GetSettingValueForUser<bool>(NotificationSettingNames.ReceiveNotifications, subscription.TenantId, subscription.UserId))
                        {
                            invalidSubscriptions[subscription.Id] = subscription;
                        }
                    }
                }

                subscriptions.RemoveAll(s => invalidSubscriptions.ContainsKey(s.Id));

                //Get user ids
                userIds = subscriptions
                    .Select(s => new UserIdentifier(s.TenantId, s.UserId))
                    .ToList();
            }

            if (!notificationInfo.ExcludedUserIds.IsNullOrEmpty())
            {
                //Exclude specified users.
                var excludedUserIds = notificationInfo
                    .ExcludedUserIds
                    .Split(",")
                    .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                    .ToList();

                userIds.RemoveAll(uid => excludedUserIds.Any(euid => euid.Equals(uid)));
            }

            return userIds.ToArray();
        }

        private static int?[] GetTenantIds(NotificationInfo notificationInfo)
        {
            if (notificationInfo.TenantIds.IsNullOrEmpty())
            {
                return null;
            }

            return notificationInfo
                .TenantIds
                .Split(",")
                .Select(tenantIdAsStr => tenantIdAsStr == "null" ? (int?)null : (int?)tenantIdAsStr.To<int>())
                .ToArray();
        }

        [UnitOfWork]
        protected virtual async Task<List<UserNotification>> SaveUserNotifications(UserIdentifier[] users, NotificationInfo notificationInfo)
        {
            var userNotifications = new List<UserNotification>();

            var tenantGroups = users.GroupBy(user => user.TenantId);
            foreach (var tenantGroup in tenantGroups)
            {
                using (this._unitOfWorkManager.Current.SetTenantId(tenantGroup.Key))
                {
                    var tenantNotificationInfo = new TenantNotificationInfo(tenantGroup.Key, notificationInfo);
                    await this._notificationStore.InsertTenantNotificationAsync(tenantNotificationInfo);
                    await this._unitOfWorkManager.Current.SaveChangesAsync(); //To get tenantNotification.Id.

                    var tenantNotification = tenantNotificationInfo.ToTenantNotification();

                    foreach (var user in tenantGroup)
                    {
                        var userNotification = new UserNotificationInfo
                        {
                            TenantId = tenantGroup.Key,
                            UserId = user.UserId,
                            TenantNotificationId = tenantNotificationInfo.Id
                        };

                        await this._notificationStore.InsertUserNotificationAsync(userNotification);
                        userNotifications.Add(userNotification.ToUserNotification(tenantNotification));
                    }

                    await this.CurrentUnitOfWork.SaveChangesAsync(); //To get Ids of the notifications
                }
            }

            return userNotifications;
        }
    }
}