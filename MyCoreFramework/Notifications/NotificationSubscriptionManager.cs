using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Entities;
using MyCoreFramework.Json;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationSubscriptionManager"/>.
    /// </summary>
    public class NotificationSubscriptionManager : INotificationSubscriptionManager, ITransientDependency
    {
        private readonly INotificationStore _store;
        private readonly INotificationDefinitionManager _notificationDefinitionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriptionManager"/> class.
        /// </summary>
        public NotificationSubscriptionManager(INotificationStore store, INotificationDefinitionManager notificationDefinitionManager)
        {
            this._store = store;
            this._notificationDefinitionManager = notificationDefinitionManager;
        }

        public async Task SubscribeAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            if (await this.IsSubscribedAsync(user, notificationName, entityIdentifier))
            {
                return;
            }

            await this._store.InsertSubscriptionAsync(
                new NotificationSubscriptionInfo(
                    user.TenantId,
                    user.UserId,
                    notificationName,
                    entityIdentifier
                    )
                );
        }

        public async Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user)
        {
            var notificationDefinitions = (await this._notificationDefinitionManager
                .GetAllAvailableAsync(user))
                .Where(nd => nd.EntityType == null)
                .ToList();

            foreach (var notificationDefinition in notificationDefinitions)
            {
                await this.SubscribeAsync(user, notificationDefinition.Name);
            }
        }

        public async Task UnsubscribeAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            await this._store.DeleteSubscriptionAsync(
                user,
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
                );
        }
        
        // TODO: Can work only for single database approach!
        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, EntityIdentifier entityIdentifier = null)
        {
            var notificationSubscriptionInfos = await this._store.GetSubscriptionsAsync(
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
                );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(int? tenantId, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            var notificationSubscriptionInfos = await this._store.GetSubscriptionsAsync(
                new[] { tenantId },
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
                );

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public async Task<List<NotificationSubscription>> GetSubscribedNotificationsAsync(UserIdentifier user)
        {
            var notificationSubscriptionInfos = await this._store.GetSubscriptionsAsync(user);

            return notificationSubscriptionInfos
                .Select(nsi => nsi.ToNotificationSubscription())
                .ToList();
        }

        public Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            return this._store.IsSubscribedAsync(
                user,
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString()
                );
        }
    }
}