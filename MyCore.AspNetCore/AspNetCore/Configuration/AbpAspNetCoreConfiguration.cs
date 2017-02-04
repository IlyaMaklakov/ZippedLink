using System;
using System.Collections.Generic;
using System.Reflection;

using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Web.Models;

namespace MyCore.AspNetCore.Configuration
{
    public class AbpAspNetCoreConfiguration : IAbpAspNetCoreConfiguration
    {
        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public List<Type> FormBodyBindingIgnoredTypes { get; }

        public ControllerAssemblySettingList ControllerAssemblySettings { get; }

        public bool IsValidationEnabledForControllers { get; set; }

        public bool IsAuditingEnabled { get; set; }

        public bool SetNoCacheForAjaxResponses { get; set; }

        public AbpAspNetCoreConfiguration()
        {
            this.DefaultWrapResultAttribute = new WrapResultAttribute();
            this.DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            this.ControllerAssemblySettings = new ControllerAssemblySettingList();
            this.FormBodyBindingIgnoredTypes = new List<Type>();
            this.IsValidationEnabledForControllers = true;
            this.SetNoCacheForAjaxResponses = true;
            this.IsAuditingEnabled = true;
        }

        public AbpControllerAssemblySettingBuilder CreateControllersForAppServices(
            Assembly assembly,
            string moduleName = AbpControllerAssemblySetting.DefaultServiceModuleName,
            bool useConventionalHttpVerbs = true)
        {
            var setting = new AbpControllerAssemblySetting(moduleName, assembly, useConventionalHttpVerbs);
            this.ControllerAssemblySettings.Add(setting);
            return new AbpControllerAssemblySettingBuilder(setting);
        }
    }
}