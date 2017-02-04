using System;
using System.Collections.Generic;
using System.Linq;
using MyCoreFramework.JetBrains.Annotations;

namespace MyCore.AspNetCore.Configuration
{
    public class ControllerAssemblySettingList : List<AbpControllerAssemblySetting>
    {
        [CanBeNull]
        public AbpControllerAssemblySetting GetSettingOrNull(Type controllerType)
        {
            return this.FirstOrDefault(controllerSetting => controllerSetting.Assembly == controllerType.Assembly);
        }
    }
}