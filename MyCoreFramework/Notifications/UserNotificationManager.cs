using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyCoreFramework.Dependency;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Implements  <see cref="IUserNotificationManager"/>.
    /// </summary>
    public class UserNotificationManager : IUserNotificationManager, ISingletonDependency
    {
        private readonly INotificationStore _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationManager"/> class.
        /// </summary>
        public UserNotificationManager(INotificationStore store)
        {
            this._store = store;
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue)
        {
            var userNotifications = await this._store.GetUserNotificationsWithNotificationsAsync(user, state, skipCount, maxResultCount);
            return userNotifications
                .Select(un => un.ToUserNotification())
                .ToList();
        }

        public Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null)
        {
            return this._store.GetUserNotificationCountAsync(user, state);
        }

        public async Task<UserNotification> GetUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            var userNotification = await this._store.GetUserNotificationWithNotificationOrNullAsync(tenantId, userNotificationId);
            if (userNotification == null)
            {
                return null;
            }

            return userNotification.ToUserNotification();
        }

        public Task UpdateUserNotificationStateAsync(int? tenantId, Guid userNotificationId, UserNotificationState state)
        {
            return this._store.UpdateUserNotificationStateAsync(tenantId, userNotificationId, state);
        }

        public Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            return this._store.UpdateAllUserNotificationStatesAsync(user, state);
        }

        public Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId)
        {
            return this._store.DeleteUserNotificationAsync(tenantId, userNotificationId);
        }

        public Task DeleteAllUserNotificationsAsync(UserIdentifier user)
        {
            return this._store.DeleteAllUserNotificationsAsync(user);
        }
    }
}