using System;

using Castle.Core.Logging;
using Castle.MicroKernel.Registration;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Dependency.Installers;
using MyCoreFramework.JetBrains.Annotations;
using MyCoreFramework.Modules;
using MyCoreFramework.PlugIns;

namespace MyCoreFramework
{
    /// <summary>
    /// This is the main class that is responsible to start entire ABP system.
    /// Prepares dependency injection and registers core components needed for startup.
    /// It must be instantiated and initialized (see <see cref="Initialize"/>) first in an application.
    /// </summary>
    public class AbpBootstrapper : IDisposable
    {
        /// <summary>
        /// Get the startup module of the application which depends on other used modules.
        /// </summary>
        public Type StartupModule { get; }

        /// <summary>
        /// A list of plug in folders.
        /// </summary>
        public PlugInSourceList PlugInSources { get; }

        /// <summary>
        /// Gets IIocManager object used by this class.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        private AbpModuleManager _moduleManager;
        private ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</param>
        private AbpBootstrapper([NotNull] Type startupModule)
            : this(startupModule, Dependency.IocManager.Instance)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</param>
        /// <param name="iocManager">IIocManager that is used to bootstrap the ABP system</param>
        private AbpBootstrapper([NotNull] Type startupModule, [NotNull] IIocManager iocManager)
        {
            Check.NotNull(startupModule, nameof(startupModule));
            Check.NotNull(iocManager, nameof(iocManager));

            if (!typeof(AbpModule).IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(AbpModule)}.");
            }

            this.StartupModule = startupModule;
            this.IocManager = iocManager;

            this.PlugInSources = new PlugInSourceList();
            this._logger = NullLogger.Instance;
        }

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</typeparam>
        public static AbpBootstrapper Create<TStartupModule>()
            where TStartupModule : AbpModule
        {
            return new AbpBootstrapper(typeof(TStartupModule));
        }

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</typeparam>
        /// <param name="iocManager">IIocManager that is used to bootstrap the ABP system</param>
        public static AbpBootstrapper Create<TStartupModule>([NotNull] IIocManager iocManager)
            where TStartupModule : AbpModule
        {
            return new AbpBootstrapper(typeof(TStartupModule), iocManager);
        }

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</param>
        public static AbpBootstrapper Create([NotNull] Type startupModule)
        {
            return new AbpBootstrapper(startupModule);
        }

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</param>
        /// <param name="iocManager">IIocManager that is used to bootstrap the ABP system</param>
        public static AbpBootstrapper Create([NotNull] Type startupModule, [NotNull] IIocManager iocManager)
        {
            return new AbpBootstrapper(startupModule, iocManager);
        }

        /// <summary>
        /// Initializes the ABP system.
        /// </summary>
        public virtual void Initialize()
        {
            this.ResolveLogger();

            try
            {
                this.RegisterBootstrapper();
                this.IocManager.IocContainer.Install(new AbpCoreInstaller());

                this.IocManager.Resolve<AbpPlugInManager>().PlugInSources.AddRange(this.PlugInSources);
                this.IocManager.Resolve<AbpStartupConfiguration>().Initialize();

                this._moduleManager = this.IocManager.Resolve<AbpModuleManager>();
                this._moduleManager.Initialize(this.StartupModule);
                this._moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                this._logger.Fatal(ex.ToString(), ex);
                throw;
            }
        }

        private void ResolveLogger()
        {
            if (this.IocManager.IsRegistered<ILoggerFactory>())
            {
                this._logger = this.IocManager.Resolve<ILoggerFactory>().Create(typeof(AbpBootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            if (!this.IocManager.IsRegistered<AbpBootstrapper>())
            {
                this.IocManager.IocContainer.Register(
                    Component.For<AbpBootstrapper>().Instance(this)
                    );
            }
        }

        /// <summary>
        /// Disposes the ABP system.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;

            this._moduleManager?.ShutdownModules();
        }
    }
}
