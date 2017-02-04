using System.Reflection;

using Microsoft.AspNetCore.Mvc.Controllers;

using MyCore.AspNetCore.Configuration;

using MyCoreFramework.Application.Services;
using MyCoreFramework.Dependency;
using MyCoreFramework.Reflection;

namespace MyCore.AspNetCore.Mvc.Providers
{
    /// <summary>
    /// Used to add application services as controller.
    /// </summary>
    public class AbpAppServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly IIocResolver _iocResolver;

        public AbpAppServiceControllerFeatureProvider(IIocResolver iocResolver)
        {
            this._iocResolver = iocResolver;
        }

        protected override bool IsController(TypeInfo typeInfo)
        {
            var type = typeInfo.AsType();

            if (!typeof(IApplicationService).IsAssignableFrom(type) ||
                !type.IsPublic || type.IsAbstract || type.IsGenericType)
            {
                return false;
            }

            var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(type);

            if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(type))
            {
                return false;
            }

            var configuration = this._iocResolver.Resolve<AbpAspNetCoreConfiguration>().ControllerAssemblySettings.GetSettingOrNull(type);
            return configuration != null && configuration.TypePredicate(type);
        }
    }
}