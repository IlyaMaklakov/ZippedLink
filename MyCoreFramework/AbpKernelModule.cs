using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using Castle.MicroKernel.Registration;

using MyCoreFramework.Application.Features;
using MyCoreFramework.Application.Navigation;
using MyCoreFramework.Application.Services;
using MyCoreFramework.Auditing;
using MyCoreFramework.Authorization;
using MyCoreFramework.BackgroundJobs;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Configuration;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Events.Bus;
using MyCoreFramework.Localization;
using MyCoreFramework.Localization.Dictionaries;
using MyCoreFramework.Localization.Dictionaries.Xml;
using MyCoreFramework.Modules;
using MyCoreFramework.MultiTenancy;
using MyCoreFramework.Net.Mail;
using MyCoreFramework.Notifications;
using MyCoreFramework.Runtime;
using MyCoreFramework.Runtime.Caching;
using MyCoreFramework.Runtime.Remoting;
using MyCoreFramework.Runtime.Validation.Interception;
using MyCoreFramework.Threading;
using MyCoreFramework.Threading.BackgroundWorkers;
using MyCoreFramework.Timing;

namespace MyCoreFramework
{
    /// <summary>
    /// Kernel (core) module of the ABP system.
    /// No need to depend on this, it's automatically the first module always.
    /// </summary>
    public sealed class AbpKernelModule : AbpModule
    {
        public override void PreInitialize()
        {
            this.IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            this.IocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            this.IocManager.Register(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>), DependencyLifeStyle.Transient);

            this.InitializeInterceptors();

            this.AddAuditingSelectors();
            this.AddLocalizationSources();
            this.AddSettingProviders();
            this.AddUnitOfWorkFilters();
            this.ConfigureCaches();
            this.AddIgnoredTypes();
        }

        public override void Initialize()
        {
            foreach (var replaceAction in ((AbpStartupConfiguration)this.Configuration).ServiceReplaceActions.Values)
            {
                replaceAction();
            }

            this.IocManager.IocContainer.Install(new EventBusInstaller(this.IocManager));

            this.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }

        public override void PostInitialize()
        {
            this.RegisterMissingComponents();

            this.IocManager.Resolve<SettingDefinitionManager>().Initialize();
            this.IocManager.Resolve<FeatureManager>().Initialize();
            this.IocManager.Resolve<PermissionManager>().Initialize();
            this.IocManager.Resolve<LocalizationManager>().Initialize();
            this.IocManager.Resolve<NotificationDefinitionManager>().Initialize();
            this.IocManager.Resolve<NavigationManager>().Initialize();

            if (this.Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                var workerManager = this.IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Start();
                workerManager.Add(this.IocManager.Resolve<IBackgroundJobManager>());
            }
        }

        public override void Shutdown()
        {
            if (this.Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                this.IocManager.Resolve<IBackgroundWorkerManager>().StopAndWaitToStop();
            }
        }

        private void InitializeInterceptors()
        {
            ValidationInterceptorRegistrar.Initialize(this.IocManager);
            AuditingInterceptorRegistrar.Initialize(this.IocManager);
            UnitOfWorkRegistrar.Initialize(this.IocManager);
            AuthorizationInterceptorRegistrar.Initialize(this.IocManager);
        }

        private void AddUnitOfWorkFilters()
        {
            this.Configuration.UnitOfWork.RegisterFilter(AbpDataFilters.SoftDelete, true);
            this.Configuration.UnitOfWork.RegisterFilter(AbpDataFilters.MustHaveTenant, true);
            this.Configuration.UnitOfWork.RegisterFilter(AbpDataFilters.MayHaveTenant, true);
        }

        private void AddSettingProviders()
        {
            this.Configuration.Settings.Providers.Add<LocalizationSettingProvider>();
            this.Configuration.Settings.Providers.Add<EmailSettingProvider>();
            this.Configuration.Settings.Providers.Add<NotificationSettingProvider>();
            this.Configuration.Settings.Providers.Add<TimingSettingProvider>();
        }

        private void AddAuditingSelectors()
        {
            this.Configuration.Auditing.Selectors.Add(
                new NamedTypeSelector(
                    "Abp.ApplicationServices",
                    type => typeof(IApplicationService).IsAssignableFrom(type)
                )
            );
        }

        private void AddLocalizationSources()
        {
            this.Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    MyCoreConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(), "Abp.Localization.Sources.AbpXmlSource"
                    )));
        }

        private void ConfigureCaches()
        {
            this.Configuration.Caching.Configure(AbpCacheNames.ApplicationSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8);
            });

            this.Configuration.Caching.Configure(AbpCacheNames.TenantSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(60);
            });

            this.Configuration.Caching.Configure(AbpCacheNames.UserSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(20);
            });
        }

        private void AddIgnoredTypes()
        {
            var commonIgnoredTypes = new[]
            {
                typeof(Stream),
                typeof(Expression)
            };

            foreach (var ignoredType in commonIgnoredTypes)
            {
                this.Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
                this.Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }

            var validationIgnoredTypes = new[] { typeof(Type) };
            foreach (var ignoredType in validationIgnoredTypes)
            {
                this.Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }

        private void RegisterMissingComponents()
        {
            if (!this.IocManager.IsRegistered<IGuidGenerator>())
            {
                this.IocManager.IocContainer.Register(
                    Component
                        .For<IGuidGenerator, SequentialGuidGenerator>()
                        .Instance(SequentialGuidGenerator.Instance)
                );
            }

            this.IocManager.RegisterIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            this.IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>(DependencyLifeStyle.Singleton);
            this.IocManager.RegisterIfNot<IPermissionChecker, NullPermissionChecker>(DependencyLifeStyle.Singleton);
            this.IocManager.RegisterIfNot<IRealTimeNotifier, NullRealTimeNotifier>(DependencyLifeStyle.Singleton);
            this.IocManager.RegisterIfNot<INotificationStore, NullNotificationStore>(DependencyLifeStyle.Singleton);
            this.IocManager.RegisterIfNot<IUnitOfWorkFilterExecuter, NullUnitOfWorkFilterExecuter>(DependencyLifeStyle.Singleton);
            this.IocManager.RegisterIfNot<IClientInfoProvider, NullClientInfoProvider>(DependencyLifeStyle.Singleton);
            this.IocManager.RegisterIfNot<ITenantStore, NullTenantStore>(DependencyLifeStyle.Singleton);
            this.IocManager.RegisterIfNot<ITenantResolverCache, NullTenantResolverCache>(DependencyLifeStyle.Singleton);

            if (this.Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                this.IocManager.RegisterIfNot<IBackgroundJobStore, InMemoryBackgroundJobStore>(DependencyLifeStyle.Singleton);
            }
            else
            {
                this.IocManager.RegisterIfNot<IBackgroundJobStore, NullBackgroundJobStore>(DependencyLifeStyle.Singleton);
            }
        }
    }
}