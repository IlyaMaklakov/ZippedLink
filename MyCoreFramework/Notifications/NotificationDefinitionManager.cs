using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using MyCoreFramework.Application.Features;
using MyCoreFramework.Authorization;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationDefinitionManager"/>.
    /// </summary>
    internal class NotificationDefinitionManager : INotificationDefinitionManager, ISingletonDependency
    {
        private readonly INotificationConfiguration _configuration;
        private readonly IocManager _iocManager;

        private readonly IDictionary<string, NotificationDefinition> _notificationDefinitions;

        public NotificationDefinitionManager(
            IocManager iocManager,
            INotificationConfiguration configuration)
        {
            this._configuration = configuration;
            this._iocManager = iocManager;

            this._notificationDefinitions = new Dictionary<string, NotificationDefinition>();
        }

        public void Initialize()
        {
            var context = new NotificationDefinitionContext(this);

            foreach (var providerType in this._configuration.Providers)
            {
                this._iocManager.RegisterIfNot(providerType, DependencyLifeStyle.Transient); //TODO: Needed?
                using (var provider = this._iocManager.ResolveAsDisposable<NotificationProvider>(providerType))
                {
                    provider.Object.SetNotifications(context);
                }
            }
        }

        public void Add(NotificationDefinition notificationDefinition)
        {
            if (this._notificationDefinitions.ContainsKey(notificationDefinition.Name))
            {
                throw new AbpInitializationException("There is already a notification definition with given name: " + notificationDefinition.Name + ". Notification names must be unique!");
            }

            this._notificationDefinitions[notificationDefinition.Name] = notificationDefinition;
        }

        public NotificationDefinition Get(string name)
        {
            var definition = this.GetOrNull(name);
            if (definition == null)
            {
                throw new AbpException("There is no notification definition with given name: " + name);
            }

            return definition;
        }

        public NotificationDefinition GetOrNull(string name)
        {
            return this._notificationDefinitions.GetOrDefault(name);
        }

        public IReadOnlyList<NotificationDefinition> GetAll()
        {
            return this._notificationDefinitions.Values.ToImmutableList();
        }

        public async Task<bool> IsAvailableAsync(string name, UserIdentifier user)
        {
            var notificationDefinition = this.GetOrNull(name);
            if (notificationDefinition == null)
            {
                return true;
            }

            if (notificationDefinition.FeatureDependency != null)
            {
                using (var featureDependencyContext = this._iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.TenantId = user.TenantId;

                    if (!await notificationDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object))
                    {
                        return false;
                    }
                }
            }

            if (notificationDefinition.PermissionDependency != null)
            {
                using (var permissionDependencyContext = this._iocManager.ResolveAsDisposable<PermissionDependencyContext>())
                {
                    permissionDependencyContext.Object.User = user;

                    if (!await notificationDefinition.PermissionDependency.IsSatisfiedAsync(permissionDependencyContext.Object))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(UserIdentifier user)
        {
            var availableDefinitions = new List<NotificationDefinition>();

            using (var permissionDependencyContext = this._iocManager.ResolveAsDisposable<PermissionDependencyContext>())
            {
                permissionDependencyContext.Object.User = user;

                using (var featureDependencyContext = this._iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.TenantId = user.TenantId;

                    foreach (var notificationDefinition in this.GetAll())
                    {
                        if (notificationDefinition.PermissionDependency != null &&
                            !await notificationDefinition.PermissionDependency.IsSatisfiedAsync(permissionDependencyContext.Object))
                        {
                            continue;
                        }

                        if (user.TenantId.HasValue &&
                            notificationDefinition.FeatureDependency != null &&
                            !await notificationDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object))
                        {
                            continue;
                        }

                        availableDefinitions.Add(notificationDefinition);
                    }
                }
            }

            return availableDefinitions.ToImmutableList();
        }
    }
}