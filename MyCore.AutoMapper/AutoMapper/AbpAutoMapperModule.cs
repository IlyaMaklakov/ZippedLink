using System.Reflection;

using AutoMapper;

using Castle.MicroKernel.Registration;

using MyCoreFramework;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Localization;
using MyCoreFramework.Modules;
using MyCoreFramework.Reflection;

using IObjectMapper = MyCoreFramework.ObjectMapping.IObjectMapper;

namespace MyCore.AutoMapper
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpAutoMapperModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        private static bool _createdMappingsBefore;

        private static readonly object SyncObj = new object();

        public AbpAutoMapperModule(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            this.IocManager.Register<IAbpAutoMapperConfiguration, AbpAutoMapperConfiguration>();

            Configuration.ReplaceService<IObjectMapper, AutoMapperObjectMapper>();

            this.Configuration.Modules.AbpAutoMapper().Configurators.Add(this.CreateCoreMappings);
        }

        public override void PostInitialize()
        {
            this.CreateMappings();

            this.IocManager.IocContainer.Register(
                Component.For<IMapper>().Instance(Mapper.Instance).LifestyleSingleton()
            );
        }

        public void CreateMappings()
        {
            lock (SyncObj)
            {
                //We should prevent duplicate mapping in an application, since Mapper is static.
                if (_createdMappingsBefore)
                {
                    return;
                }

                Mapper.Initialize(configuration =>
                {
                    this.FindAndAutoMapTypes(configuration);
                    foreach (var configurator in this.Configuration.Modules.AbpAutoMapper().Configurators)
                    {
                        configurator(configuration);
                    }
                });

                _createdMappingsBefore = true;
            }
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var types = this._typeFinder.Find(type =>
                    type.IsDefined(typeof(AutoMapAttribute)) ||
                    type.IsDefined(typeof(AutoMapFromAttribute)) ||
                    type.IsDefined(typeof(AutoMapToAttribute))
            );

            this.Logger.DebugFormat("Found {0} classes define auto mapping attributes", types.Length);

            foreach (var type in types)
            {
                this.Logger.Debug(type.FullName);
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void CreateCoreMappings(IMapperConfigurationExpression configuration)
        {
            var localizationContext = this.IocManager.Resolve<ILocalizationContext>();

            configuration.CreateMap<ILocalizableString, string>().ConvertUsing(ls => ls?.Localize(localizationContext));
            configuration.CreateMap<LocalizableString, string>().ConvertUsing(ls => ls == null ? null : localizationContext.LocalizationManager.GetString(ls));
        }
    }
}
