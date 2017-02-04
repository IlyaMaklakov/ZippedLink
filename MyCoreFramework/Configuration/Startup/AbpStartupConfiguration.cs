using System;
using System.Collections.Generic;

using MyCoreFramework.Application.Features;
using MyCoreFramework.Auditing;
using MyCoreFramework.BackgroundJobs;
using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Events.Bus;
using MyCoreFramework.Notifications;
using MyCoreFramework.Resources.Embedded;
using MyCoreFramework.Runtime.Caching.Configuration;

namespace MyCoreFramework.Configuration.Startup
{
    /// <summary>
    /// This class is used to configure ABP and modules on startup.
    /// </summary>
    internal class AbpStartupConfiguration : DictionaryBasedConfig, IAbpStartupConfiguration
    {
        /// <summary>
        /// Reference to the IocManager.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        public ILocalizationConfiguration Localization { get; private set; }

        /// <summary>
        /// Used to configure authorization.
        /// </summary>
        public IAuthorizationConfiguration Authorization { get; private set; }

        /// <summary>
        /// Used to configure validation.
        /// </summary>
        public IValidationConfiguration Validation { get; private set; }

        /// <summary>
        /// Used to configure settings.
        /// </summary>
        public ISettingsConfiguration Settings { get; private set; }

        /// <summary>
        /// Gets/sets default connection string used by ORM module.
        /// It can be name of a connection string in application's config file or can be full connection string.
        /// </summary>
        public string DefaultNameOrConnectionString { get; set; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="ModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        public IModuleConfigurations Modules { get; private set; }

        /// <summary>
        /// Used to configure unit of work defaults.
        /// </summary>
        public IUnitOfWorkDefaultOptions UnitOfWork { get; private set; }

        /// <summary>
        /// Used to configure features.
        /// </summary>
        public IFeatureConfiguration Features { get; private set; }

        /// <summary>
        /// Used to configure background job system.
        /// </summary>
        public IBackgroundJobConfiguration BackgroundJobs { get; private set; }

        /// <summary>
        /// Used to configure notification system.
        /// </summary>
        public INotificationConfiguration Notifications { get; private set; }

        /// <summary>
        /// Used to configure navigation.
        /// </summary>
        public INavigationConfiguration Navigation { get; private set; }

        /// <summary>
        /// Used to configure <see cref="IEventBus"/>.
        /// </summary>
        public IEventBusConfiguration EventBus { get; private set; }

        /// <summary>
        /// Used to configure auditing.
        /// </summary>
        public IAuditingConfiguration Auditing { get; private set; }

        public ICachingConfiguration Caching { get; private set; }

        /// <summary>
        /// Used to configure multi-tenancy.
        /// </summary>
        public IMultiTenancyConfig MultiTenancy { get; private set; }

        public Dictionary<Type, Action> ServiceReplaceActions { get; private set; }

        public IEmbeddedResourcesConfiguration EmbeddedResources { get; private set; }

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        public AbpStartupConfiguration(IIocManager iocManager)
        {
            this.IocManager = iocManager;
        }

        public void Initialize()
        {
            this.Localization = this.IocManager.Resolve<ILocalizationConfiguration>();
            this.Modules = this.IocManager.Resolve<IModuleConfigurations>();
            this.Features = this.IocManager.Resolve<IFeatureConfiguration>();
            this.Navigation = this.IocManager.Resolve<INavigationConfiguration>();
            this.Authorization = this.IocManager.Resolve<IAuthorizationConfiguration>();
            this.Validation = this.IocManager.Resolve<IValidationConfiguration>();
            this.Settings = this.IocManager.Resolve<ISettingsConfiguration>();
            this.UnitOfWork = this.IocManager.Resolve<IUnitOfWorkDefaultOptions>();
            this.EventBus = this.IocManager.Resolve<IEventBusConfiguration>();
            this.MultiTenancy = this.IocManager.Resolve<IMultiTenancyConfig>();
            this.Auditing = this.IocManager.Resolve<IAuditingConfiguration>();
            this.Caching = this.IocManager.Resolve<ICachingConfiguration>();
            this.BackgroundJobs = this.IocManager.Resolve<IBackgroundJobConfiguration>();
            this.Notifications = this.IocManager.Resolve<INotificationConfiguration>();
            this.EmbeddedResources = this.IocManager.Resolve<IEmbeddedResourcesConfiguration>();

            this.ServiceReplaceActions = new Dictionary<Type, Action>();
        }

        public void ReplaceService(Type type, Action replaceAction)
        {
            this.ServiceReplaceActions[type] = replaceAction;
        }

        public T Get<T>()
        {
            return this.GetOrCreate(typeof(T).FullName, () => this.IocManager.Resolve<T>());
        }
    }
}