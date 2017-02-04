using System.Reflection;

using Castle.MicroKernel.Registration;

using MyCore.EntityFramework.Repositories;
using MyCore.EntityFramework.Uow;

using MyCoreFramework;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Modules;
using MyCoreFramework.Reflection;

namespace MyCore.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in EntityFramework.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpEntityFrameworkModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        public AbpEntityFrameworkModule(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            this.Configuration.ReplaceService<IUnitOfWorkFilterExecuter>(() =>
            {
                this.IocManager.IocContainer.Register(
                    Component
                    .For<IUnitOfWorkFilterExecuter, IEfUnitOfWorkFilterExecuter>()
                    .ImplementedBy<EfDynamicFiltersUnitOfWorkFilterExecuter>()
                    .LifestyleTransient()
                );
            });
        }

        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            this.IocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider<>))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient()
                );

            this.RegisterGenericRepositoriesAndMatchDbContexes();
        }

        private void RegisterGenericRepositoriesAndMatchDbContexes()
        {
            var dbContextTypes =
                this._typeFinder.Find(type =>
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    typeof(AbpDbContext).IsAssignableFrom(type)
                    );

            if (dbContextTypes.IsNullOrEmpty())
            {
                this.Logger.Warn("No class found derived from AbpDbContext.");
                return;
            }

            using (var repositoryRegistrar = this.IocManager.ResolveAsDisposable<IEntityFrameworkGenericRepositoryRegistrar>())
            {
                foreach (var dbContextType in dbContextTypes)
                {
                    this.Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);
                    repositoryRegistrar.Object.RegisterForDbContext(dbContextType, this.IocManager);
                }
            }

            using (var dbContextMatcher = this.IocManager.ResolveAsDisposable<IDbContextTypeMatcher>())
            {
                dbContextMatcher.Object.Populate(dbContextTypes);
            }
        }
    }
}
