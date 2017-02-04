using System;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MyCore.AspNetCore.Configuration
{
    public class AbpControllerAssemblySettingBuilder : IAbpControllerAssemblySettingBuilder
    {
        private readonly AbpControllerAssemblySetting _setting;

        public AbpControllerAssemblySettingBuilder(AbpControllerAssemblySetting setting)
        {
            this._setting = setting;
        }

        public AbpControllerAssemblySettingBuilder Where(Func<Type, bool> predicate)
        {
            this._setting.TypePredicate = predicate;
            return this;
        }

        public AbpControllerAssemblySettingBuilder ConfigureControllerModel(Action<ControllerModel> configurer)
        {
            this._setting.ControllerModelConfigurer = configurer;
            return this;
        }
    }
}