using System;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MyCore.AspNetCore.Configuration
{
    public interface IAbpControllerAssemblySettingBuilder
    {
        AbpControllerAssemblySettingBuilder Where(Func<Type, bool> predicate);

        AbpControllerAssemblySettingBuilder ConfigureControllerModel(Action<ControllerModel> configurer);
    }
}